function alternarClasse(elementos, classe, gravarCookie=true) {
    elementos.forEach(function (elementoId) {
        const elemento = document.getElementById(elementoId);
        if (elemento) {
            elemento.classList.toggle(classe);
            if (gravarCookie) {
                document.cookie = "classe|" + elementoId + "|" + classe + "=" + elemento.classList.contains(classe) + "; path=/";
            }
        }
    });
}

function _aguarde(mostrar=true) {
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



