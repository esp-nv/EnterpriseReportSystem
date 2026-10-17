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

    function evaluatePasswordSubmitMatrix() {
        if (!submitBtn || !currentPassInput || !newPassInput || !confirmPassInput) return;

        const currentVal = currentPassInput.value;
        const newVal = newPassInput.value;
        const confirmVal = confirmPassInput.value;

        let shouldBlock = false;

        // Блокираме, ако е попълнено някое от новите полета, но липсва текущата парола
        if ((newVal.length > 0 || confirmVal.length > 0) && currentVal.length === 0) {
            shouldBlock = true;
        }

        // Блокираме, ако новата парола и повторението се разминават
        if (newVal !== confirmVal) {
            shouldBlock = true;
        }

        // Прилагаме заключването и променяме визуалния стил на бутона
        submitBtn.disabled = shouldBlock;
        if (shouldBlock) {
            submitBtn.classList.remove("btn-primary");
            submitBtn.classList.add("btn-secondary");
            submitBtn.style.cursor = "not-allowed";
        } else {
            submitBtn.classList.remove("btn-secondary");
            submitBtn.classList.add("btn-primary");
            submitBtn.style.cursor = "pointer";
        }
    }

    // Закачаме слушатели за всяко натискане на клавиш (input event) в трите полета
    if (currentPassInput) currentPassInput.addEventListener("input", evaluatePasswordSubmitMatrix);
    if (newPassInput) newPassInput.addEventListener("input", evaluatePasswordSubmitMatrix);
    if (confirmPassInput) confirmPassInput.addEventListener("input", evaluatePasswordSubmitMatrix);

});
