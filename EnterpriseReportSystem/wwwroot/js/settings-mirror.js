document.addEventListener("DOMContentLoaded", function () {
    // =========================================================================
    // 1. ИЗНЕСЕНА ФАБРИЧНА ЛОГИКА ЗА ЛЕВИТЕ ТАБОВЕ (ОТ ДЪНОТО НА HTML ФАЙЛА)
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
    // 2. УПРАВЛЕНИЕ НА ТРИТЕ НОВИ ВЕРТИКАЛНИ БУТОНА В СЕКЦИЯ 3
    // =========================================================================
    var propsBtnGeneral = document.getElementById('props-btn-general');
    var propsBtnCustom = document.getElementById('props-btn-custom');
    var propsBtnExternal = document.getElementById('props-btn-external');

    var section3Buttons = [propsBtnGeneral, propsBtnCustom, propsBtnExternal];

    function handleSection3ButtonClick(clickedBtn) {
        if (!clickedBtn) return;

        // Изчистваме и приглушаваме абсолютно всички три бутона накуп
        section3Buttons.forEach(function (btn) {
            if (btn) {
                btn.classList.remove('btn-primary', 'text-white');
                btn.classList.add('btn-dark', 'text-muted');
                btn.style.backgroundColor = '';
                btn.style.borderColor = '';
            }
        });

        // Светваме в синьо само и единствено кликнатия бутон
        clickedBtn.classList.remove('btn-dark', 'text-muted');
        clickedBtn.classList.add('btn-primary', 'text-white');
        clickedBtn.style.backgroundColor = '#007acc';
        clickedBtn.style.borderColor = '#0062a3';
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
