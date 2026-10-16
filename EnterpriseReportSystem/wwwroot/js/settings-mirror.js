document.addEventListener("DOMContentLoaded", function () {
    // =========================================================================
    // 1. СИНХРОНИЗАЦИЯ НА ЛЕВИТЕ СТРАНИЧНИ ТАБОВЕ (Visual Studio Ефект)
    // =========================================================================
    var pillButtons = document.querySelectorAll('#vs-tab-system .nav-link');
    pillButtons.forEach(function (btn) {
        btn.addEventListener('shown.bs.tab', function () {
            pillButtons.forEach(function (b) {
                b.style.backgroundColor = 'transparent';
                b.classList.add('text-muted');
                b.classList.remove('text-white');
            });
            this.style.backgroundColor = '#37373d';
            this.classList.remove('text-muted');
            this.classList.add('text-white');
        });
    });

    // =========================================================================
    // 2. ИНТЕЛЕГЕНТНО УПРАВЛЕНИЕ НА ТРИТЕ БУТОНА ВЪТРЕ В СЕКЦИЯ 3 (В СГЪВАЕМИТЕ КУТИИ)
    // =========================================================================
    document.addEventListener('click', function (event) {
        if (event.target && (event.target.id === 'props-btn-general' || event.target.id === 'props-btn-custom' || event.target.id === 'props-btn-external')) {
            var clickedBtn = event.target;
            var parentContainer = clickedBtn.closest('.card');
            if (!parentContainer) return;

            var section3Buttons = parentContainer.querySelectorAll('#props-btn-general, #props-btn-custom, #props-btn-external');

            section3Buttons.forEach(function (btn) {
                btn.style.backgroundColor = '#333333';
                btn.style.setProperty('border-color', '#6c757d', 'important');
                btn.classList.add('text-muted');
                btn.classList.remove('text-white');
            });

            clickedBtn.style.backgroundColor = '#007acc';
            clickedBtn.style.setProperty('border-color', '#0062a3', 'important');
            clickedBtn.classList.remove('text-muted');
            clickedBtn.classList.add('text-white');
        }
    });

    // =========================================================================
    // 3. ДОБАВЕНО: АВТОМАТИЧНО ЗАКЛЮЧВАНЕ НА КАТАЛОГА ЗА ТЕМИ (НОВОУВЕДЕНИЕ)
    // =========================================================================
    const radioFactory = document.getElementById("radioFactoryStyle");
    const radioCustom = document.getElementById("radioCustomStyle");
    const themeDropdown = document.getElementById("themeIdDropdown");
    const labelTheme = document.getElementById("labelThemeDropdown");

    function evalThemeLockMatrix() {
        if (radioFactory && radioFactory.checked) {
            // Ако е избран фабричен Bootstrap -> Напълно изолираме и заключваме падащото меню
            if (themeDropdown) {
                themeDropdown.disabled = true;
                themeDropdown.style.opacity = "0.4";
                themeDropdown.value = "";
            }
            if (labelTheme) {
                labelTheme.className = "form-label fw-bold text-muted mb-1 text-decoration-line-through";
            }
        } else if (radioCustom && radioCustom.checked) {
            // Ако е избрана персонализирана тема -> Светваме менюто в синьо и отключваме файловете
            if (themeDropdown) {
                themeDropdown.disabled = false;
                themeDropdown.style.opacity = "1";
            }
            if (labelTheme) {
                labelTheme.className = "form-label fw-bold text-info mb-1";
            }
        }
    }

    // Закачаме чисти събития за промяна към радио бутоните
    if (radioFactory) radioFactory.addEventListener("change", evalThemeLockMatrix);
    if (radioCustom) radioCustom.addEventListener("change", evalThemeLockMatrix);

    // Изпълняваме проверката веднага на старта за първоначално отчитане на профила
    evalThemeLockMatrix();
});

// =========================================================================
// 4. ДОБАВЕНО: ПРЕВКЛЮЧВАНЕ НА ВИДИМОСТТА НА ПАРОЛАТА (SHOW/HIDE EYE EFFECT)
// =========================================================================
const togglePasswordBtn = document.getElementById("togglePasswordVisibilityBtn");
const passwordInput = document.getElementById("operatorPasswordInput");
const eyeIcon = document.getElementById("passwordEyeIcon");

if (togglePasswordBtn && passwordInput && eyeIcon) {
    togglePasswordBtn.addEventListener("click", function () {
        // Проверяваме текущия тип на полето
        const isPassword = passwordInput.getAttribute("type") === "password";

        if (isPassword) {
            passwordInput.setAttribute("type", "text");
            eyeIcon.classList.remove("bi-eye");
            eyeIcon.classList.add("bi-eye-slash"); // Зачеркнато око
            togglePasswordBtn.classList.remove("text-secondary");
            togglePasswordBtn.classList.add("text-primary"); // Светва в синьо при активиран текст
        } else {
            passwordInput.setAttribute("type", "password");
            eyeIcon.classList.remove("bi-eye-slash");
            eyeIcon.classList.add("bi-eye"); // Нормално око
            togglePasswordBtn.classList.remove("text-primary");
            togglePasswordBtn.classList.add("text-secondary");
        }
    });
}
