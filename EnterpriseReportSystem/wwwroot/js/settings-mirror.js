// Интегрирано управление на потребителския интерфейс по модела на StudentManagement
document.addEventListener("DOMContentLoaded", function () {

    // 1. СИНХРОНИЗАЦИЯ НА ЛЕВИТЕ СТРАНИЧНИ ТАБОВЕ
    document.querySelectorAll('#vs-tab-system .nav-link').forEach(btn => {
        btn.addEventListener('shown.bs.tab', function () {
            document.querySelectorAll('#vs-tab-system .nav-link').forEach(b => {
                b.classList.add('text-muted');
                b.classList.remove('text-white');
            });
            // Премахнат твърдия цвят #37373d - сега се управлява на чисто от CSS темата!
            this.classList.remove('text-muted');
            this.classList.add('text-white');
        });
    });

    // 2. ПРЕВКЛЮЧВАНЕ НА ВИДИМОСТТА НА ПАРОЛАТА (SHOW/HIDE EYE EFFECT)
    const togglePasswordBtn = document.getElementById("togglePasswordVisibilityBtn");
    const passwordInput = document.getElementById("operatorPasswordInput");
    const eyeIcon = document.getElementById("passwordEyeIcon");

    if (togglePasswordBtn && passwordInput && eyeIcon) {
        togglePasswordBtn.addEventListener("click", function () {
            const isPassword = passwordInput.getAttribute("type") === "password";
            if (isPassword) {
                passwordInput.setAttribute("type", "text");
                eyeIcon.className = "bi bi-eye-slash";
                togglePasswordBtn.classList.remove("text-secondary");
                togglePasswordBtn.classList.add("text-primary");
            } else {
                passwordInput.setAttribute("type", "password");
                eyeIcon.className = "bi bi-eye";
                togglePasswordBtn.classList.remove("text-primary");
                togglePasswordBtn.classList.add("text-secondary");
            }
        });
    }

    // 3. АВТОМАТИЧНО УПРАВЛЕНИЕ И ФИЛТРАЦИЯ НА ТЕМИТЕ (МАТРИЦА НА СЪОТВЕТСТВИЕТО)
    const radioFactory = document.getElementById("radioFactoryStyle");
    const radioCustom = document.getElementById("radioCustomStyle");
    const themeDropdown = document.getElementById("themeIdDropdown");
    const modeDark = document.getElementById("modeDarkRadio");
    const modeLight = document.getElementById("modeLightRadio");

    function evaluateThemeInterfaceMatrix() {
        if (!themeDropdown) return;

        const isFactoryMode = radioFactory && radioFactory.checked;
        const isCurrentSystemDark = modeDark && modeDark.checked;

        // Контрол на достъпа до падащото меню
        themeDropdown.disabled = isFactoryMode;
        themeDropdown.style.opacity = isFactoryMode ? "0.5" : "1";

        // Динамично филтриране и заключване на опциите спрямо контраста
        Array.from(themeDropdown.options).forEach(option => {
            const themeDarkData = option.getAttribute("data-dark");
            if (!themeDarkData) return;

            // Сравняваме булевите низове "true"/"false"
            const isCompatible = themeDarkData === isCurrentSystemDark.toString();

            option.disabled = !isCompatible;
            if (!isCompatible) {
                option.classList.add("text-muted", "disabled");
                if (option.selected) {
                    option.selected = false;
                    if (radioFactory) radioFactory.checked = true;
                }
            } else {
                option.classList.remove("text-muted", "disabled");
            }
        });

        // Ако сме върнали на фабричен режим принудително, преизчисляваме състоянието
        if (radioFactory && radioFactory.checked && themeDropdown.disabled === false) {
            themeDropdown.disabled = true;
            themeDropdown.style.opacity = "0.5";
        }
    }

    // Закачаме чисти събития по примера на GitHub site.js
    if (radioFactory) radioFactory.addEventListener("change", evaluateThemeInterfaceMatrix);
    if (radioCustom) radioCustom.addEventListener("change", evaluateThemeInterfaceMatrix);
    if (modeDark) modeDark.addEventListener("change", evaluateThemeInterfaceMatrix);
    if (modeLight) modeLight.addEventListener("change", evaluateThemeInterfaceMatrix);

    // Изпълняваме първоначален баланс при зареждане на екрана
    evaluateThemeInterfaceMatrix();
});
