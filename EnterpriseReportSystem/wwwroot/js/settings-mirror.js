document.addEventListener("DOMContentLoaded", function () {

    // =========================================================================
    // 1. СИНХРОНИЗАЦИЯ НА СТРАНИЧНИТЕ ТАБОВЕ (ЧИСТИ BOOTSTRAP КЛАСОВЕ)
    // =========================================================================
    document.querySelectorAll('#vs-tab-system .nav-link').forEach(btn => {
        btn.addEventListener('shown.bs.tab', function () {
            document.querySelectorAll('#vs-tab-system .nav-link').forEach(b => {
                b.classList.add('text-muted');
                b.classList.remove('text-white');
            });
            this.classList.remove('text-muted');
            this.classList.add('text-white');
        });
    });

    // =========================================================================
    // 2. ОБЩ ЧЕКБОКС "ПОКАЖИ ПАРОЛИТЕ" (УПРАВЛЯВА И ТРИТЕ ПОЛЕТА ЕДНОВРЕМЕННО)
    // =========================================================================
    const showAllPasswordsCheckbox = document.getElementById("globalShowPasswordsCheckbox");
    const currentPassInput = document.getElementById("inputCurrentPass");
    const newPassInput = document.getElementById("inputNewPass");
    const confirmPassInput = document.getElementById("inputConfirmPass");

    if (showAllPasswordsCheckbox) {
        showAllPasswordsCheckbox.addEventListener("change", function () {
            // Определяме типа спрямо състоянието на чекбокса
            const targetType = this.checked ? "text" : "password";

            // Превключваме и трите полета в синхрон
            if (currentPassInput) currentPassInput.setAttribute("type", targetType);
            if (newPassInput) newPassInput.setAttribute("type", targetType);
            if (confirmPassInput) confirmPassInput.setAttribute("type", targetType);
        });
    }

    // =========================================================================
    // 3. АВТОМАТИЧНО УПРАВЛЕНИЕ И ФИЛТРАЦИЯ НА ТЕМИТЕ (МАТРИЦА НА СЪОТВЕТСТВИЕТО)
    // =========================================================================
    const radioFactory = document.getElementById("radioFactoryStyle");
    const radioCustom = document.getElementById("radioCustomStyle");
    const themeDropdown = document.getElementById("themeIdDropdown");
    const modeDark = document.getElementById("modeDarkRadio");
    const modeLight = document.getElementById("modeLightRadio");

    function evaluateThemeInterfaceMatrix() {
        if (!themeDropdown) return;

        const isFactoryMode = radioFactory && radioFactory.checked;
        const isCurrentSystemDark = modeDark && modeDark.checked;

        themeDropdown.disabled = isFactoryMode;
        themeDropdown.style.opacity = isFactoryMode ? "0.5" : "1";

        Array.from(themeDropdown.options).forEach(option => {
            const themeDarkData = option.getAttribute("data-dark");
            if (!themeDarkData) return;

            const isCompatible = themeDarkData === isCurrentSystemDark.toString();
            option.disabled = !isCompatible;

            if (!isCompatible) {
                option.classList.add("text-muted", "disabled");
                // Ако скриваме темата, премахваме отметката от името ѝ
                option.text = option.text.replace("✓ ", "");

                if (option.selected) {
                    option.selected = false;
                    if (radioFactory) radioFactory.checked = true;
                }
            } else {
                option.classList.remove("text-muted", "disabled");
            }
        });

        if (radioFactory && radioFactory.checked && themeDropdown.disabled === false) {
            themeDropdown.disabled = true;
            themeDropdown.style.opacity = "0.5";
        }
    }

    if (radioFactory) radioFactory.addEventListener("change", evaluateThemeInterfaceMatrix);
    if (radioCustom) radioCustom.addEventListener("change", evaluateThemeInterfaceMatrix);
    if (modeDark) modeDark.addEventListener("change", evaluateThemeInterfaceMatrix);
    if (modeLight) modeLight.addEventListener("change", evaluateThemeInterfaceMatrix);

    // Изпълняваме първоначален баланс при зареждане на екрана
    evaluateThemeInterfaceMatrix();

    // =========================================================================
    // 5. ЖИВА БЛОКИРОВКА НА БУТОНА ЗА ЗАПИС ПРИ НЕСЪВПАДАЩИ ПАРОЛИ
    // =========================================================================
    const submitBtn = document.querySelector("button[type='submit']");

    // =========================================================================
    // 5. ГЛОБАЛЕН ТРАКЕР ЗА ПРОМЯНА НА НАСТРОЙКИТЕ И ЗАКЛЮЧВАНЕ НА ЗАПИСА
    // =========================================================================
    const settingsForm = document.querySelector("form");
    const masterSubmitBtn = document.getElementById("mainSubmitSettingsBtn");
    const masterResetBtn = document.getElementById("mainResetSettingsBtn");

    // Запечатваме първоначалното състояние на формата веднага при зареждане на уеб страницата
    const initialFormStateMap = new Map();

    if (settingsForm) {
        const inputs = settingsForm.querySelectorAll("input, select");
        inputs.forEach(input => {
            if (input.id === "globalShowPasswordsCheckbox" || input.id === "showPasswordCheckbox") return;

            if (input.type === "checkbox" || input.type === "radio") {
                initialFormStateMap.set(input, input.checked);
            } else {
                initialFormStateMap.set(input, input.value);
            }
        });
    }

    function evaluateGlobalFormChangesMatrix() {
        if (!masterSubmitBtn) return;

        let hasAnyRealChange = false;

        // Сканираме всички полета за разлика спрямо първоначалните стойности
        initialFormStateMap.forEach((initialValue, input) => {
            let currentValue = (input.type === "checkbox" || input.type === "radio") ? input.checked : input.value;

            if (currentValue !== initialValue) {
                hasAnyRealChange = true;
            }
        });

        // СПЕЦИАЛНА ЗАЩИТА ЗА ПАРОЛИТЕ: Ако се въвеждат нови пароли, текущата задължително трябва да присъства и да съвпадат
        if (currentPassInput && newPassInput && confirmPassInput) {
            const currentVal = currentPassInput.value.trim();
            const newVal = newPassInput.value.trim();
            const confirmVal = confirmPassInput.value.trim();

            if (newVal.length > 0 || confirmVal.length > 0) {
                if (currentVal.length === 0 || newVal !== confirmVal) {
                    hasAnyRealChange = false; // Блокираме отключването при невалидни пароли
                }
            }
        }

        // АКТУАЛИЗИРАМЕ СТАТУСА НА БУТОНА ЗА ЗАПИС
        masterSubmitBtn.disabled = !hasAnyRealChange;
        if (!hasAnyRealChange) {
            masterSubmitBtn.className = "btn btn-secondary w-100 fw-bold";
            masterSubmitBtn.style.cursor = "not-allowed";
        } else {
            masterSubmitBtn.className = "btn btn-primary w-100 fw-bold";
            masterSubmitBtn.style.cursor = "pointer";
        }
    }

    // Закачаме живи слушатели за промяна върху абсолютно всяко поле в системата
    if (settingsForm) {
        settingsForm.querySelectorAll("input, select").forEach(input => {
            if (input.id === "globalShowPasswordsCheckbox" || input.id === "showPasswordCheckbox") return;

            input.addEventListener("input", evaluateGlobalFormChangesMatrix);
            input.addEventListener("change", evaluateGlobalFormChangesMatrix);
        });
    }

    // ЛОГИКА ЗА БУТОНА "ОТКАЗ ОТ ПРОМЕНИ"
    if (masterResetBtn && settingsForm) {
        masterResetBtn.addEventListener("click", function () {
            // Връщаме фабричните стойности на всяко поле от запазената ни карта
            initialFormStateMap.forEach((initialValue, input) => {
                if (input.type === "checkbox" || input.type === "radio") {
                    input.checked = initialValue;
                } else {
                    input.value = initialValue;
                }
                // Ръчно задействаме събитие за опресняване на интерфейсните матрици
                input.dispatchEvent(new Event('change'));
            });

            // Изчистваме ръчно полетата за нови пароли, ако са били въвеждани
            if (currentPassInput) currentPassInput.value = "";
            if (newPassInput) newPassInput.value = "";
            if (confirmPassInput) confirmPassInput.value = "";

            evaluateGlobalFormChangesMatrix();
        });
    }

    // Изпълняваме начален контрол при стартиране на уеб екрана
    evaluateGlobalFormChangesMatrix();


});
