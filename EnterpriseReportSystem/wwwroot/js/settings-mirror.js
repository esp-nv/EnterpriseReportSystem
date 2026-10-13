document.addEventListener("DOMContentLoaded", function () {
    // Селектори от конфигурационната форма (вляво)
    const layoutSelector = document.getElementById("layoutSelector");
    const themeSelector = document.getElementById("themeSelector");
    const bgColorPicker = document.getElementById("bgColorPicker");
    const accentColorPicker = document.getElementById("accentColorPicker");
    const textColorPicker = document.getElementById("textColorPicker");

    // Селектори от интерактивното огледало (вдясно)
    const mirrorDashboardLayout = document.getElementById("mirrorDashboardLayout");
    const mirrorRibbonLayout = document.getElementById("mirrorRibbonLayout");
    const previewMirrorFrame = document.getElementById("previewMirrorFrame");
    const mirrorSidebar = document.getElementById("mirrorSidebar");
    const mirrorDashboardContent = document.getElementById("mirrorDashboardContent");
    const mirrorRibbonHeader = document.getElementById("mirrorRibbonHeader");
    const mirrorRibbonContent = document.getElementById("mirrorRibbonContent");
    const mirrorGearIcon = document.getElementById("mirrorGearIcon");
    const mirrorRibbonGear = document.getElementById("mirrorRibbonGear");
    const mirrorBadge = document.getElementById("mirrorBadge");
    const mirrorCryptoCard = document.getElementById("mirrorCryptoCard");
    const mirrorRibbonCryptoCard = document.getElementById("mirrorRibbonCryptoCard");

    // 1. ДИНАМИЧНО ПРЕВКЛЮЧВАНЕ НА МАКЕТА (RIBBON / DASHBOARD)
    function updateLayoutMirror() {
        if (!layoutSelector || !mirrorDashboardLayout || !mirrorRibbonLayout) return;

        if (layoutSelector.value === "RIBBON") {
            mirrorDashboardLayout.style.setProperty("display", "none", "important");
            mirrorRibbonLayout.style.setProperty("display", "block", "important");
        } else {
            mirrorDashboardLayout.style.setProperty("display", "flex", "important");
            mirrorRibbonLayout.style.setProperty("display", "none", "important");
        }
    }

    // 2. ЖИВО ПРЕБОЯДИСВАНЕ СПРЯМО 4-ТЕ СЪСТОЯНИЯ НА ТЕМИТЕ
    function updateThemeAndColorsMirror() {
        if (!themeSelector) return;
        const selectedMode = themeSelector.value;

        // Палитрите се отключват ЕДИНСТВЕНО и само при режим "custom"
        const isCustomMode = (selectedMode === "custom");
        const pickers = [bgColorPicker, accentColorPicker, textColorPicker];
        pickers.forEach(picker => {
            if (picker) {
                picker.disabled = !isCustomMode;
                picker.style.cursor = isCustomMode ? "pointer" : "not-allowed";
            }
        });

        if (selectedMode === "light") {
            if (previewMirrorFrame) { previewMirrorFrame.style.background = "#f8f9fa"; previewMirrorFrame.style.color = "#212529"; }
            if (mirrorSidebar) mirrorSidebar.style.background = "#e9ecef";
            if (mirrorDashboardContent) mirrorDashboardContent.style.background = "#ffffff";
            if (mirrorRibbonHeader) mirrorRibbonHeader.style.background = "#e9ecef";
            if (mirrorRibbonContent) mirrorRibbonContent.style.background = "#ffffff";
            if (mirrorGearIcon) mirrorGearIcon.style.color = "#0d6efd";
            if (mirrorRibbonGear) mirrorRibbonGear.style.color = "#0d6efd";
            if (mirrorBadge) { mirrorBadge.style.backgroundColor = "#212529"; mirrorBadge.style.color = "#ffffff"; }
            if (mirrorCryptoCard) mirrorCryptoCard.style.background = "#f1f3f5";
            if (mirrorRibbonCryptoCard) mirrorRibbonCryptoCard.style.background = "#f1f3f5";
        }
        else if (selectedMode === "dark") {
            if (previewMirrorFrame) { previewMirrorFrame.style.background = "#141414"; previewMirrorFrame.style.color = "#e0e0e0"; }
            if (mirrorSidebar) mirrorSidebar.style.background = "#1c1c1c";
            if (mirrorDashboardContent) mirrorDashboardContent.style.background = "#222222";
            if (mirrorRibbonHeader) mirrorRibbonHeader.style.background = "#1c1c1c";
            if (mirrorRibbonContent) mirrorRibbonContent.style.background = "#222222";
            if (mirrorGearIcon) mirrorGearIcon.style.color = "#ff5500";
            if (mirrorRibbonGear) mirrorRibbonGear.style.color = "#ff5500";
            if (mirrorBadge) { mirrorBadge.style.backgroundColor = "#ff5500"; mirrorBadge.style.color = "#141414"; }
            if (mirrorCryptoCard) mirrorCryptoCard.style.background = "rgba(255,255,255,0.03)";
            if (mirrorRibbonCryptoCard) mirrorRibbonCryptoCard.style.background = "rgba(255,255,255,0.03)";
        }
        else if (selectedMode === "custom") {
            const bgVal = bgColorPicker ? bgColorPicker.value : "#141414";
            const accentVal = accentColorPicker ? accentColorPicker.value : "#ff5500";
            const textVal = textColorPicker ? textColorPicker.value : "#e0e0e0";

            if (previewMirrorFrame) { previewMirrorFrame.style.background = bgVal; previewMirrorFrame.style.color = textVal; }
            if (mirrorSidebar) mirrorSidebar.style.background = bgVal;
            if (mirrorDashboardContent) mirrorDashboardContent.style.background = bgVal;
            if (mirrorRibbonHeader) mirrorRibbonHeader.style.background = bgVal;
            if (mirrorRibbonContent) mirrorRibbonContent.style.background = bgVal;
            if (mirrorGearIcon) mirrorGearIcon.style.color = accentVal;
            if (mirrorRibbonGear) mirrorRibbonGear.style.color = accentVal;
            if (mirrorBadge) { mirrorBadge.style.backgroundColor = accentVal; mirrorBadge.style.color = "#ffffff"; }
            if (mirrorCryptoCard) mirrorCryptoCard.style.background = "rgba(255,255,255,0.05)";
            if (mirrorRibbonCryptoCard) mirrorRibbonCryptoCard.style.background = "rgba(255,255,255,0.05)";
        }
        else if (selectedMode === "external") {
            // Луксозен брандов изглед за външния зареден user.css файл
            if (previewMirrorFrame) { previewMirrorFrame.style.background = "#0f172a"; previewMirrorFrame.style.color = "#38bdf8"; }
            if (mirrorSidebar) mirrorSidebar.style.background = "#1e293b";
            if (mirrorDashboardContent) mirrorDashboardContent.style.background = "#0f172a";
            if (mirrorRibbonHeader) mirrorRibbonHeader.style.background = "#1e293b";
            if (mirrorRibbonContent) mirrorRibbonContent.style.background = "#0f172a";
            if (mirrorGearIcon) mirrorGearIcon.style.color = "#38bdf8";
            if (mirrorRibbonGear) mirrorRibbonGear.style.color = "#38bdf8";
            if (mirrorBadge) { mirrorBadge.style.backgroundColor = "#38bdf8"; mirrorBadge.style.color = "#0f172a"; }
            if (mirrorCryptoCard) mirrorCryptoCard.style.background = "rgba(255,255,255,0.05)";
            if (mirrorRibbonCryptoCard) mirrorRibbonCryptoCard.style.background = "rgba(255,255,255,0.05)";
        }
    }

    // Закачане на чисти слушатели за събития (Event Listeners)
    if (layoutSelector) layoutSelector.addEventListener("change", updateLayoutMirror);
    if (themeSelector) themeSelector.addEventListener("change", updateThemeAndColorsMirror);

    if (bgColorPicker) bgColorPicker.addEventListener("input", updateThemeAndColorsMirror);
    if (accentColorPicker) accentColorPicker.addEventListener("input", updateThemeAndColorsMirror);
    if (textColorPicker) textColorPicker.addEventListener("input", updateThemeAndColorsMirror);

    // Първоначално стартиране при отваряне на настройките
    updateLayoutMirror();
    updateThemeAndColorsMirror();
});

// Валидация на формите
(function () {
    'use strict'
    var forms = document.querySelectorAll('.needs-validation')
    Array.prototype.slice.call(forms)
        .forEach(function (form) {
            form.addEventListener('submit', function (event) {
                if (!form.checkValidity()) {
                    event.preventDefault()
                    event.stopPropagation()
                }
                form.classList.add('was-validated')
            }, false)
        })
})();
