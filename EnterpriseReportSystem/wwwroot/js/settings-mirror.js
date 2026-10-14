document.addEventListener("DOMContentLoaded", function () {
    // 1. Селектори за левите табове и десните контейнери
    var tabTriggerElements = document.querySelectorAll('#vs-tab-system button[data-bs-toggle="pill"]');

    // 2. Инициализираме фабричния Bootstrap таб механизъм за всеки бутон
    tabTriggerElements.forEach(function (tabEl) {
        tabEl.addEventListener('click', function (event) {
            event.preventDefault();

            // Премахваме изкуствения фокус от всички бутони, за да няма застъпване на кутиите
            tabTriggerElements.forEach(function (btn) {
                btn.classList.remove('active');
                btn.setAttribute('aria-selected', 'false');
                btn.style.backgroundColor = 'transparent';
                btn.style.color = '#aaaaaa';
            });

            // Активираме на чисто само кликнатия бутон
            this.classList.add('active');
            this.setAttribute('aria-selected', 'true');

            // Задаваме фабричния сив Visual Studio цвят при избор на главен таб
            if (this.id === 'tab-general') {
                this.style.backgroundColor = '#37373d';
                this.style.color = '#ffffff';
            }

            // Задействаме реалното показване на десния панел през Bootstrap
            var tabInstance = bootstrap.Tab.getOrCreateInstance(this);
            tabInstance.show();
        });
    });

    // 3. Фабрична системна валидация на HTML формите при запис
    (function () {
        'use strict';
        var forms = document.querySelectorAll('.needs-validation');
        Array.prototype.slice.call(forms).forEach(function (form) {
            form.addEventListener('submit', function (event) {
                if (!form.checkValidity()) {
                    event.preventDefault();
                    event.stopPropagation();
                }
                form.classList.add('was-validated');
            }, false);
        });
    })();
});
