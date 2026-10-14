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
    // Използваме Event Delegation, за да уловим кликанията върху бутоните динамично
    document.addEventListener('click', function (event) {
        // Проверяваме дали кликнатият елемент е някой от трите бутона в Секция 3
        if (event.target && (event.target.id === 'props-btn-general' || event.target.id === 'props-btn-custom' || event.target.id === 'props-btn-external')) {
            var clickedBtn = event.target;

            // Намираме родителския контейнер на конкретната Секция 3
            var parentContainer = clickedBtn.closest('.card');
            if (!parentContainer) return;

            // Извличаме трите бутона в рамките само на ТАЗИ конкретна кутия
            var section3Buttons = parentContainer.querySelectorAll('#props-btn-general, #props-btn-custom, #props-btn-external');

            // Изчистваме и приглушаваме абсолютно всички три бутона накуп
            section3Buttons.forEach(function (btn) {
                btn.style.backgroundColor = '#333333';
                btn.style.setProperty('border-color', '#6c757d', 'important');
                btn.classList.add('text-muted');
                btn.classList.remove('text-white');
            });

            // Светваме ярко в синьо само и единствено кликнатия бутон
            clickedBtn.style.backgroundColor = '#007acc';
            clickedBtn.style.setProperty('border-color', '#0062a3', 'important');
            clickedBtn.classList.remove('text-muted');
            clickedBtn.classList.add('text-white');
        }
    });
});
