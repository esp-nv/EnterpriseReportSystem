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
    // 2. ПРЕВКЛЮЧВАНЕ НА ВИДИМОСТТА НА ПАРОЛАТА (Show/Hide Eye Effect)
    // =========================================================================
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

    // =========================================================================
    // 3. ЖИВО ЗАКЛЮЧВАНЕ НА COMBOBOX-А (Group Box UX контрол)
    // =========================================================================
    const radioFactory = document.getElementById("radioFactoryStyle");
    const radioCustom = document.getElementById("radioCustomStyle");
    const themeDropdown = document.getElementById("themeIdDropdown");
    const labelTheme = document.getElementById("labelThemeDropdown");

    function evalThemeLockMatrix() {
        if (radioFactory && radioFactory.checked) {
            if (themeDropdown) themeDropdown.disabled = true;
            if (labelTheme) labelTheme.className = "form-label fw-bold small text-muted text-decoration-line-through";
        } else if (radioCustom && radioCustom.checked) {
            if (themeDropdown) themeDropdown.disabled = false;
            if (labelTheme) labelTheme.className = "form-label fw-bold small text-primary";
        }
    }

    if (radioFactory) radioFactory.addEventListener("change", evalThemeLockMatrix);
    if (radioCustom) radioCustom.addEventListener("change", evalThemeLockMatrix);

    // =========================================================================
    // 4. ДОBAВЕНО: ИНТЕЛЕГЕНТЕН ЖИВ ФИЛТЪР НА ТЕМИТЕ СПРЯМО КОНТРАСТА
    // =========================================================================
    const modeDark = document.getElementById("modeDarkRadio");
    const modeLight = document.getElementById("modeLightRadio");

    function filterThemesDropdown() {
        if (!themeDropdown) return;

        // Определяме дали текущо е избран Тъмен (true) или Светъл (false) режим
        const isCurrentSystemDark = modeDark && modeDark.checked;
        const options = themeDropdown.getElementsByTagName("option");

        for (let option of options) {
            const themeDarkData = option.getAttribute("data-dark");
            if (!themeDarkData) continue;

            // Извличаме чистото оригинално име на темата (махаме старото подсещане, ако има такова)
            let baseText = option.text.replace(" (несъвместима)", "");

            if (themeDarkData === isCurrentSystemDark.toString()) {
                // ПОЗВОЛЕНА ТЕМА: Връщаме оригиналния текст и я отключваме
                option.disabled = false;
                option.text = baseText;
            } else {
                // НЕСЪВМЕСТИМА ТЕМА: Блокираме я на Disabed и подсещаме визуално
                option.disabled = true;
                option.text = baseText + " (несъвместима)";

                // FALLBACK ЗАЩИТА: Ако потребителят е бил стъпил върху нея, връщаме на Фабрична котва
                if (option.selected) {
                    option.selected = false;
                    if (radioFactory) {
                        radioFactory.checked = true;
                        evalThemeLockMatrix();
                    }
                }
            }
        }
    }
