// URL API для фирм-производителей
const API_URL = 'https://localhost:7284/api/FacturingCompanies';

// DOM-элементы
const tbody = document.getElementById('table-body');
const errorDiv = document.getElementById('error-message');
const firmNameInput = document.getElementById('firmName');
const adressInput = document.getElementById('adress');
const directorSurnameInput = document.getElementById('directorSurname');
const submitBtn = document.getElementById('submit-btn');
const cancelBtn = document.getElementById('cancel-btn');
const editIdField = document.getElementById('edit-id');
const formTitle = document.getElementById('form-title');
const totalSpan = document.getElementById('total-count');
const directorSpan = document.getElementById('director-count');

let currentEditId = null;
let cachedFirms = [];

// ---- Вспомогательные функции ----
function showError(text) {
    errorDiv.textContent = text;
    errorDiv.classList.remove('hidden');
    setTimeout(() => errorDiv.classList.add('hidden'), 5000);
}

function clearForm() {
    firmNameInput.value = '';
    adressInput.value = '';
    directorSurnameInput.value = '';
    editIdField.value = '';
    currentEditId = null;
    formTitle.textContent = 'Добавить фирму';
    submitBtn.textContent = 'Добавить';
    cancelBtn.style.display = 'none';
}

// ---- Статистика ----
function updateStats(items) {
    totalSpan.textContent = items.length;
    const withDirector = items.filter(f => f.directorSurname && f.directorSurname.trim() !== '');
    directorSpan.textContent = withDirector.length;
}

// ---- Отрисовка таблицы ----
function renderTable(items) {
    tbody.innerHTML = '';
    items.forEach(item => {
        const row = tbody.insertRow();
        row.insertCell(0).textContent = item.firmId;
        row.insertCell(1).textContent = item.firmName;
        row.insertCell(2).textContent = item.adress;
        row.insertCell(3).textContent = item.directorSurname || 'не указан';

        const actionsCell = row.insertCell(4);

        const editBtn = document.createElement('button');
        editBtn.textContent = '✏️'; editBtn.title = 'Редактировать';
        editBtn.onclick = () => {
            currentEditId = item.firmId;
            editIdField.value = item.firmId;
            firmNameInput.value = item.firmName;
            adressInput.value = item.adress;
            directorSurnameInput.value = item.directorSurname || '';
            formTitle.textContent = 'Редактировать фирму';
            submitBtn.textContent = 'Сохранить';
            cancelBtn.style.display = 'inline-block';
        };

        const deleteBtn = document.createElement('button');
        deleteBtn.textContent = '🗑️'; deleteBtn.title = 'Удалить';
        deleteBtn.onclick = () => deleteItem(item.firmId);

        actionsCell.appendChild(editBtn);
        actionsCell.appendChild(deleteBtn);
    });
}

// ---- Вычисление Id ----
function getNextFirmId() {
    if (cachedFirms.length === 0) return 1;
    const ids = cachedFirms.map(f => f.firmId).sort((a, b) => a - b);
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
        cachedFirms = await res.json();
        renderTable(cachedFirms);
        updateStats(cachedFirms);
        errorDiv.classList.add('hidden');
    } catch (err) {
        showError(`Ошибка загрузки: ${err.message}`);
    }
}


// ---- Добавление нового ----
async function createItem() {
    const newId = getNextFirmId();
    const newItem = {
        firmId: freeIdFirm,
        firmName: firmNameInput.value.trim(),
        adress: adressInput.value.trim(),
        directorSurname: directorSurnameInput.value.trim() || null
    };

    if (!newItem.firmName || !newItem.adress) {
        showError('Название и адрес обязательны');
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
                showError('Фирма с таким ID уже существует');
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
        firmId: id,
        firmName: firmNameInput.value.trim(),
        adress: adressInput.value.trim(),
        directorSurname: directorSurnameInput.value.trim() || null
    };

    if (!updated.firmName || !updated.adress) {
        showError('Название и адрес обязательны');
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
            return false;;
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
    if (!confirm('Удалить эту фирму?')) return;

    try {
        const response = await fetch(`${API_URL}/${id}`, { method: 'DELETE' });

        if (response.status === 404) {
            showError('Фирма не найдена (возможно, уже удалена)');
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

// ---- Инициализация и обработчики событий ----
submitBtn.addEventListener('click', onSubmit);
cancelBtn.addEventListener('click', onCancel);

// Загружаем данные при старте
loadData();