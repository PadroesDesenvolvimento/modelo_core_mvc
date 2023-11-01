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

function aplicarClassesDeCookies() {
    var cookies = document.cookie.split("; ");
    cookies.forEach(cookie => {
        if (cookie.startsWith("classe|")) {
            var nome = cookie.split("=")[0];
            var elementoId = nome.split("|")[1];
            var classe = nome.split("|")[2];
            var elemento = document.getElementById(elementoId);
            if (elemento) {
                var ativar = cookie.split("=")[1].split(";")[0];
                if (elemento.classList.contains(classe)) {
                    if (ativar === "false") {
                        elemento.classList.remove(classe);
                    };
                }
                else {
                    if (ativar === "true") {
                        elemento.classList.add(classe);
                    }
                }
            }
        }
    });
}

aplicarClassesDeCookies();
