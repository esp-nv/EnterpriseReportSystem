document.addEventListener("DOMContentLoaded", function () {
    // 1. ВЪЗСТАНОВЯВАНЕ: Фабрично превключване на активните бутони отляво за Visual Studio ефект
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

    // 2. ВЪЗСТАНОВЯВАНЕ: Изпитаният скрипт за управление на трите десни бутона в Секция 3
    var propsBtnGeneral = document.getElementById('props-btn-general');
    var propsBtnCustom = document.getElementById('props-btn-custom');
    var propsBtnExternal = document.getElementById('props-btn-external');

    var section3Buttons = [propsBtnGeneral, propsBtnCustom, propsBtnExternal];

    function handleSection3ButtonClick(clickedBtn) {
        if (!clickedBtn) return;

        // Връщаме оригиналните тъмни конзолни класове и стилове
        section3Buttons.forEach(function (btn) {
            if (btn) {
                btn.style.backgroundColor = '#333333';
                btn.style.setProperty('border-color', '#6c757d', 'important');
                btn.classList.add('text-muted');
                btn.classList.remove('text-white');
            }
        });

        // Светваме ярко в синьо само и единствено кликнатия бутон
        clickedBtn.style.backgroundColor = '#007acc';
        clickedBtn.style.setProperty('border-color', '#0062a3', 'important');
        clickedBtn.classList.remove('text-muted');
        clickedBtn.classList.add('text-white');
    }

    if (propsBtnGeneral) {
        propsBtnGeneral.addEventListener('click', function () {
            handleSection3ButtonClick(this);
        });
    }

    if (propsBtnCustom) {
        propsBtnCustom.addEventListener('click', function () {
            handleSection3ButtonClick(this);
        });
    }

    if (propsBtnExternal) {
        propsBtnExternal.addEventListener('click', function () {
            handleSection3ButtonClick(this);
        });
    }
});
