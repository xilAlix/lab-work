// URL API для производства продуктов
const API_URL = 'https://localhost:7284/api/FoodProductions';

const tbody = document.getElementById('table-body');
const errorDiv = document.getElementById('error-message');
const firmIdInput = document.getElementById('firmId');
const productIdInput = document.getElementById('productId');
const productionVolumeInput = document.getElementById('productionVolume');
const submitBtn = document.getElementById('submit-btn');
const cancelBtn = document.getElementById('cancel-btn');
const formTitle = document.getElementById('form-title');
const totalSpan = document.getElementById('total-count');
const avgVolumeSpan = document.getElementById('avg-volume');

let currentFirmId = null;
let currentProductId = null;
let currentVolume = null;

// ---- Вспомогательные функции ----
function showError(text) {
    errorDiv.textContent = text;
    errorDiv.classList.remove('hidden');
    setTimeout(() => errorDiv.classList.add('hidden'), 5000);
}

function clearForm() {
    firmIdInput.value = '';
    productIdInput.value = '';
    productionVolumeInput.value = '';
    currentFirmId = null;
    currentProductId = null;
    currentVolume = null;
    formTitle.textContent = 'Добавить запись о производстве';
    submitBtn.textContent = 'Добавить';
    cancelBtn.style.display = 'none';

    firmIdInput.disabled = false;
    productIdInput.disabled = false;
    productionVolumeInput.disabled = false;
}

// ---- Статистика ----
async function updateStats() {
    try {
        const response = await fetch(API_URL);
        if (!response.ok) throw new Error('Не удалось загрузить статистику');
        const data = await response.json();
        totalSpan.textContent = data.length;
        if (data.length > 0) {
            const sum = data.reduce((acc, cur) => acc + cur.productionVolume, 0);
            avgVolumeSpan.textContent = (sum / data.length).toFixed(2);
        } else {
            avgVolumeSpan.textContent = '0';
        }
    } catch (err) {
        console.error('Ошибка статистики:', err);
    }
}

// ---- Отрисовка таблицы ----
async function renderTable() {
    try {
        const response = await fetch(API_URL);
        if (!response.ok) throw new Error(`HTTP ${response.status}`);
        const items = await response.json();
        tbody.innerHTML = ''; // очистка

        items.forEach(item => {
            const row = tbody.insertRow();
            row.insertCell(0).textContent = item.firmId;
            row.insertCell(1).textContent = item.productId;
            row.insertCell(2).textContent = item.productionVolume;

            const actionsCell = row.insertCell(3);
            //const editBtn = document.createElement('button');
            //editBtn.textContent = '✏️';
            //editBtn.title = 'Редактировать';
            //editBtn.onclick = () => {
            //    currentFirmId = item.firmId;
            //    currentProductId = item.productId;
            //    currentVolume = item.productionVolume;
            //    firmIdInput.value = item.firmId;
            //    productIdInput.value = item.productId;
            //    productionVolumeInput.value = item.productionVolume;

            //    firmIdInput.disabled = true;
            //    productIdInput.disabled = true;
            //    productionVolumeInput.disabled = true;

            //    formTitle.textContent = 'Редактировать запись';
            //    submitBtn.textContent = 'Сохранить';
            //    cancelBtn.style.display = 'inline-block';
            //};

            const deleteBtn = document.createElement('button');
            deleteBtn.textContent = '🗑️';
            deleteBtn.title = 'Удалить';
            deleteBtn.onclick = () => deleteItem(item.firmId, item.productId, item.productionVolume);

            //actionsCell.appendChild(editBtn);
            actionsCell.appendChild(deleteBtn);
        });

        await updateStats();
    } catch (err) {
        showError(`Ошибка загрузки: ${err.message}`);
    }
}

// ---- Добавление нового ----
async function createItem() {
    const newItem = {
        firmId: parseInt(firmIdInput.value),
        productId: parseInt(productIdInput.value),
        productionVolume: parseFloat(productionVolumeInput.value)
    };

    if (!newItem.firmId || !newItem.productId || isNaN(newItem.productionVolume)) {
        showError('Все поля обязательны');
        return false;
    }

    try {
        const response = await fetch(API_URL, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(newItem)
        });

        if (response.status === 409) {
            showError('Такая запись уже существует');
            return false;
        }

        if (response.status === 400) {
            showError('Неверный запрос');
            return false;
        }

        if (!response.ok) throw new Error(`HTTP ${response.status}`);

        clearForm();
        await renderTable();
        return true;
    } catch (err) {
        showError(`Не удалось добавить: ${err.message}`);
        return false;
    }
}

// ---- Обновление существующего ----
async function updateItem(firmId, productId, volume) {
    const updated = {
        firmId: parseInt(firmIdInput.value),
        productId: parseInt(productIdInput.value),
        productionVolume: parseFloat(productionVolumeInput.value)
    };

    try {
        const response = await fetch(`${API_URL}/${firmId}/${productId}/${volume}`, {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(updated)
        });

        if (response.status === 400) {
            showError('Неверный запрос (несоответствие ключей)');
            return false;
        }

        if (response.status === 404) {
            showError('Запись не найдена');
            return false;
        }

        if (!response.ok) throw new Error(`HTTP ${response.status}`);

        clearForm();
        await renderTable();
        return true;
    } catch (err) {
        showError(`Ошибка обновления: ${err.message}`);
        return false;
    }
}

// ---- Удаление ----
async function deleteItem(firmId, productId, volume) {
    if (!confirm('Удалить эту запись?')) return;

    try {
        const response = await fetch(`${API_URL}/${firmId}/${productId}/${volume}`, { method: 'DELETE' });

        if (response.status === 404) {
            showError('Запись не найдена');
        } else if (!response.ok) {
            throw new Error(`HTTP ${response.status}`);
        }

        await renderTable();
    } catch (err) {
        showError(`Ошибка удаления: ${err.message}`);
    }
}

// ---- Обработчик кнопки "Добавить/Сохранить" ----
async function onSubmit() {
    if (currentFirmId !== null) {
        await updateItem(currentFirmId, currentProductId, currentVolume);
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
renderTable();