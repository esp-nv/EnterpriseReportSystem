function updateThemeAndColorsMirror() {
    if (!themeSelector) return;
    const selectedMode = themeSelector.value;

    // Контрол на палитрите - Отключват се ЕДИНСТВЕНО при избор на "custom"
    const isCustomMode = (selectedMode === "custom");
    const pickers = [bgColorPicker, accentColorPicker, textColorPicker];
    pickers.forEach(picker => {
        if (picker) {
            picker.disabled = !isCustomMode;
            picker.style.cursor = isCustomMode ? "pointer" : "not-allowed";
        }
    });

    // ДИНАМИЧНО ПРЕБОЯДИСВАНЕ НА ЖИВОТО ОГЛЕДАЛО СПРЯМО 4-ТЕ СЪСТОЯНИЯ
    if (selectedMode === "light") {
        if (previewMirrorFrame) { previewMirrorFrame.style.background = "#f8f9fa"; previewMirrorFrame.style.color = "#212529"; }
        if (mirrorSidebar) mirrorSidebar.style.background = "#e9ecef";
        if (mirrorDashboardContent) mirrorDashboardContent.style.background = "#ffffff";
        if (mirrorBadge) { mirrorBadge.style.backgroundColor = "#212529"; mirrorBadge.style.color = "#ffffff"; }
    }
    else if (selectedMode === "dark") {
        if (previewMirrorFrame) { previewMirrorFrame.style.background = "#141414"; previewMirrorFrame.style.color = "#e0e0e0"; }
        if (mirrorSidebar) mirrorSidebar.style.background = "#1c1c1c";
        if (mirrorDashboardContent) mirrorDashboardContent.style.background = "#222222";
        if (mirrorBadge) { mirrorBadge.style.backgroundColor = "#ff5500"; mirrorBadge.style.color = "#141414"; }
    }
    else if (selectedMode === "custom") {
        const bgVal = bgColorPicker ? bgColorPicker.value : "#141414";
        const accentVal = accentColorPicker ? accentColorPicker.value : "#ff5500";
        const textVal = textColorPicker ? textColorPicker.value : "#e0e0e0";

        if (previewMirrorFrame) { previewMirrorFrame.style.background = bgVal; previewMirrorFrame.style.color = textVal; }
        if (mirrorSidebar) mirrorSidebar.style.background = bgVal;
        if (mirrorDashboardContent) mirrorDashboardContent.style.background = bgVal;
        if (mirrorBadge) { mirrorBadge.style.backgroundColor = accentVal; mirrorBadge.style.color = "#ffffff"; }
    }
    else if (selectedMode === "external") {
        // Ефект за външния зареден файл - наливаме бърз брандов контраст
        if (previewMirrorFrame) { previewMirrorFrame.style.background = "#0f172a"; previewMirrorFrame.style.color = "#38bdf8"; }
        if (mirrorSidebar) mirrorSidebar.style.background = "#1e293b";
        if (mirrorDashboardContent) mirrorDashboardContent.style.background = "#0f172a";
        if (mirrorBadge) { mirrorBadge.style.backgroundColor = "#38bdf8"; mirrorBadge.style.color = "#0f172a"; }
    }
}
