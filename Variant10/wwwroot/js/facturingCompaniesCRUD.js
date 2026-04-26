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
async function updateStats() {
    try {
        const response = await fetch(API_URL);
        if (!response.ok) throw new Error('Не удалось загрузить статистику');
        const data = await response.json();
        totalSpan.textContent = data.length;
        const withDirector = data.filter(a => a.directorSurname && a.directorSurname.trim() !== '');
        directorSpan.textContent = withDirector.length;
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
            row.insertCell(1).textContent = item.firmName;
            row.insertCell(2).textContent = item.adress;
            row.insertCell(3).textContent = item.directorSurname || 'не указан';

            const actionsCell = row.insertCell(4);
            const editBtn = document.createElement('button');
            editBtn.textContent = '✏️';
            editBtn.title = 'Редактировать';
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
            deleteBtn.textContent = '🗑️';
            deleteBtn.title = 'Удалить';
            deleteBtn.onclick = () => deleteItem(item.firmId);

            actionsCell.appendChild(editBtn);
            actionsCell.appendChild(deleteBtn);
        });

        await updateStats();
    } catch (err) {
        showError(`Ошибка загрузки: ${err.message}`);
    }
}

// ---- Вычисление Id ----
async function getFirstFreeFirmId() {
    try {
        const response = await fetch(API_URL);
        if (!response.ok) return 1;
        const items = await response.json();
        const ids = items.map(item => item.firmId).sort((a, b) => a - b);
        let freeId = 1;
        for (const id of ids) {
            if (id === freeId) freeId++;
            else if (id > freeId) break;
        }
        return freeId;
    } catch {
        return 1;
    }
}

// ---- Добавление нового ----
async function createItem() {
    const freeIdFirm = await getFirstFreeFirmId();
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
        await renderTable();
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
        await renderTable();
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

        await renderTable();
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
renderTable();