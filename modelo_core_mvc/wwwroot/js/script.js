function setCookie(name, value, days=7) {
    var expires = "";
    if (days > 0) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

function getCookie(name) {
    let nameEQ = name + "=";
    let ca = document.cookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

function alternarClasse(elementos, classe, gravarCookie = true, days = 7) {
    elementos.forEach(function (elementoId) {
        const elemento = document.getElementById(elementoId);
        if (elemento) {
            elemento.classList.toggle(classe);
            if (gravarCookie) {
                const cookieName = "classe|" + elementoId + "|" + classe;
                const cookieValue = elemento.classList.contains(classe);
                setCookie(cookieName, cookieValue, days);
            }
        }
    });
}

function _aguarde(mostrar = true) {
    var elemento = document.getElementById('_aguarde');
    if (elemento) {
        if (elemento.classList.contains('oculto')) {
            if (mostrar) {
                elemento.classList.remove('oculto');
            };
        }
        else {
            if (!mostrar) {
                elemento.classList.add('oculto');
            }
        }
    }
}

window.addEventListener('load', _aguarde(false));
function _selecionar() {
    const menuItems = document.querySelectorAll('.botaoMenu');

    // Restaurar o item selecionado do cookie
    const itemSelecionado = getCookie('itemSelecionado');
    if (itemSelecionado) {
        const itemMenu = Array.from(menuItems).find(item => item.outerText === itemSelecionado);
        if (itemMenu) {
            itemMenu.classList.add('selecionado');
        }
    }

    menuItems.forEach(item => {
        item.addEventListener('click', function () {
            // Remover a classe "selecionado" de todos os itens
            menuItems.forEach(i => i.classList.remove('selecionado'));

            // Adicionar a classe "selecionado" ao item clicado
            this.classList.add('selecionado');

            // Salvar o estado do item selecionado em um cookie
            const action = this.outerText;
            if (action) {
                setCookie('itemSelecionado', action, 0);
            }
        });
    });
}

document.addEventListener('DOMContentLoaded', _selecionar);