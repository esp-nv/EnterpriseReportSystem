document.addEventListener("DOMContentLoaded", function () {
    // 1. Намираме всички леви бутони, които имат заложена дясна страница (data-bs-target)
    var navButtons = document.querySelectorAll('#bd-docs-nav button[data-bs-target]');
    var tabPanes = document.querySelectorAll('.tab-content .tab-pane');

    navButtons.forEach(function (button) {
        button.addEventListener('click', function (event) {
            event.preventDefault();

            // Прочитаме кой десен панел трябва да се отвори (напр. #content-elements)
            var targetId = this.getAttribute('data-bs-target');
            if (!targetId) return;

            // СТЪПКА А: Премахваме активния цвят от абсолютно всички леви бутони
            navButtons.forEach(function (btn) {
                btn.classList.remove('active');
                btn.style.backgroundColor = 'transparent';
                btn.style.color = '#858585'; // Сиво по подразбиране
            });

            // СТЪПКА Б: Светваме в бяло само текущо избрания бутон
            this.classList.add('active');
            this.style.color = '#ffffff';

            // Ако е избран първия главен таб, му връщаме сивия Visual Studio фон
            if (this.id === 'tab-general') {
                this.style.backgroundColor = '#37373d';
            }

            // СТЪПКА В: Скриваме всички десни десктоп панели
            tabPanes.forEach(function (pane) {
                pane.classList.remove('show', 'active');
            });

            // СТЪПКА Г: Отваряме чисто и ярко избрания панел вдясно
            var targetPane = document.querySelector(targetId);
            if (targetPane) {
                targetPane.classList.add('show', 'active');
            }
        });
    });
});
