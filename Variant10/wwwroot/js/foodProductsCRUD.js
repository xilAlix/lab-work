// URL API для продуктов питания
const API_URL = 'https://localhost:7284/api/FoodProducts';

// DOM-элементы
const tbody = document.getElementById('table-body');
const errorDiv = document.getElementById('error-message');
const titleInput = document.getElementById('title');
const productGroupInput = document.getElementById('productGroup');
const packageTypeInput = document.getElementById('packageType');
const submitBtn = document.getElementById('submit-btn');
const cancelBtn = document.getElementById('cancel-btn');
const editIdField = document.getElementById('edit-id');
const formTitle = document.getElementById('form-title');
const totalSpan = document.getElementById('total-count');
const packageSpan = document.getElementById('package-count');

let currentEditId = null;
let cachedProducts = [];

// ---- Вспомогательные функции ----
function showError(text) {
    errorDiv.textContent = text;
    errorDiv.classList.remove('hidden');
    setTimeout(() => errorDiv.classList.add('hidden'), 5000);
}

function clearForm() {
    titleInput.value = '';
    productGroupInput.value = '';
    packageTypeInput.value = '';
    editIdField.value = '';
    currentEditId = null;
    formTitle.textContent = 'Добавить продукт';
    submitBtn.textContent = 'Добавить';
    cancelBtn.style.display = 'none';
}

// ---- Статистика ----
function updateStats(items) {
    totalSpan.textContent = items.length;
    const withPackage = items.filter(p => p.packageType && p.packageType.trim() !== '');
    packageSpan.textContent = withPackage.length;
}

// ---- Отрисовка таблицы ----
function renderTable(items) {
    tbody.innerHTML = '';
    items.forEach(item => {
        const row = tbody.insertRow();
        row.insertCell(0).textContent = item.id;
        row.insertCell(1).textContent = item.title;
        row.insertCell(2).textContent = item.productGroup;
        row.insertCell(3).textContent = item.packageType || 'не указано';

        const actionsCell = row.insertCell(4);

        const editBtn = document.createElement('button');
        editBtn.textContent = '✏️'; editBtn.title = 'Редактировать';
        editBtn.onclick = () => {
            currentEditId = item.id;
            editIdField.value = item.id;
            titleInput.value = item.title;
            productGroupInput.value = item.productGroup;
            packageTypeInput.value = item.packageType || '';
            formTitle.textContent = 'Редактировать продукт';
            submitBtn.textContent = 'Сохранить';
            cancelBtn.style.display = 'inline-block';
        };

        const deleteBtn = document.createElement('button');
        deleteBtn.textContent = '🗑️'; deleteBtn.title = 'Удалить';
        deleteBtn.onclick = () => deleteItem(item.id);

        actionsCell.appendChild(editBtn);
        actionsCell.appendChild(deleteBtn);
    });
}

// ---- Вычисление ID ----
function getNextProductId() {
    if (cachedProducts.length === 0) return 1;
    const ids = cachedProducts.map(p => p.id).sort((a, b) => a - b);
    let freeId = 1;
    for (const id of ids) {
        if (id === freeId) freeId++;
        else if (id > freeId) break;
    }
    return freeId;
}

// ---- Запрос ----
async function loadData() {
    try {
        const res = await fetch(API_URL);
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        cachedProducts = await res.json();
        renderTable(cachedProducts);
        updateStats(cachedProducts);
        errorDiv.classList.add('hidden');
    } catch (err) {
        showError(`Ошибка загрузки: ${err.message}`);
    }
}


// ---- Добавление нового ----
async function createItem() {
    const newId = getNextProductId();
    const newItem = {
        id: freeIdProduct,
        title: titleInput.value.trim(),
        productGroup: productGroupInput.value.trim(),
        packageType: packageTypeInput.value.trim() || null
    };

    if (!newItem.title || !newItem.productGroup) {
        showError('Название и группа продукта обязательны');
        return false;
    }

    try {
        const response = await fetch(API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(newItem)
        });

        if (!response.ok) {
            if (response.status === 409) {
                showError('Продукт с таким ID уже существует');
            } else {
                throw new Error(`HTTP ${response.status}`);
            }
            return false;
        }

        clearForm();
        await loadData();
        return true;
    } catch (err) {
        showError(`Не удалось добавить: ${err.message}`);
        return false;
    }
}

// ---- Обновление существующего ----
async function updateItem(id) {
    const updated = {
        id: id,
        title: titleInput.value.trim(),
        productGroup: productGroupInput.value.trim(),
        packageType: packageTypeInput.value.trim() || null
    };

    if (!updated.title || !updated.productGroup) {
        showError('Название и группа продукта обязательны');
        return false;
    }

    try {
        const response = await fetch(`${API_URL}/${id}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(updated)
        });

        if (response.status === 400) {
            showError('Неверный запрос (несоответствие ID)');
            return false;
        }

        if (!response.ok) throw new Error(`HTTP ${response.status}`);

        clearForm();
        await loadData();
        return true;
    } catch (err) {
        showError(`Ошибка обновления: ${err.message}`);
        return false;
    }
}

// ---- Удаление ----
async function deleteItem(id) {
    if (!confirm('Удалить этот продукт?')) return;

    try {
        const response = await fetch(`${API_URL}/${id}`, { method: 'DELETE' });

        if (response.status === 404) {
            showError('Продукт не найден (возможно, уже удалён)');
        } else if (!response.ok) {
            throw new Error(`HTTP ${response.status}`);
        }

        await loadData();
    } catch (err) {
        showError(`Ошибка удаления: ${err.message}`);
    }
}

// ---- Обработчик кнопки "Добавить/Сохранить" ----
async function onSubmit() {
    if (currentEditId !== null) {
        await updateItem(currentEditId);
    } else {
        await createItem();
    }
}

// ---- Отмена редактирования ----
function onCancel() {
    clearForm();
}

// ---- Фильтры ----
async function applyFilters() {
    const sortBy = document.getElementById('filter-sort').value;
    const minId = document.getElementById('filter-min-id').value;
    const group = document.getElementById('filter-group').value.trim();

    const params = new URLSearchParams();
    if (sortBy) params.append('sortBy', sortBy);
    if (minId) params.append('minId', minId);
    if (group) params.append('group', group);

    const url = params.toString() ? `${API_URL}?${params}` : API_URL;

    try {
        const res = await fetch(url);
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        const items = await res.json();

        renderTable(items);   
        updateStats(items);  
    } catch (err) {
        showError(`Ошибка: ${err.message}`);
    }
}

function resetFilters() {
    document.getElementById('filter-sort').value = '';
    document.getElementById('filter-min-id').value = '';
    document.getElementById('filter-group').value = '';
    loadData();
}

// ---- Инициализация и обработчики событий ----
document.getElementById('apply-filters').addEventListener('click', applyFilters);
document.getElementById('reset-filters').addEventListener('click', resetFilters);

submitBtn.addEventListener('click', onSubmit);
cancelBtn.addEventListener('click', onCancel);

// Загружаем данные при старте
loadData();