function setCookie(name, value, days=7) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
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



