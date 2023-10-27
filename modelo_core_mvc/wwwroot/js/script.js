function setCookie(cookieName, cookieValue) {
    document.cookie = cookieName + "=" + cookieValue + "; path=/";
}

function deleteCookie(cookieName) {
    document.cookie = cookieName + "=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
}

function applyClassesFromCookies() {
    const classCookies = getAllClassCookies();

    for (const className in classCookies) {
        const elements = document.getElementsByClassName(className);

        if (elements.length > 0) {
            elements.forEach(element => {
                if (classCookies[className] === "true") {
                    element.classList.add(className);
                } else {
                    element.classList.remove(className);
                }
            });
        }
    }
}

function getAllClassCookies() {
    const cookies = document.cookie.split("; ");
    const classCookies = {};

    cookies.forEach(cookie => {
        if (cookie.startsWith("class_")) {
            const [name, value] = cookie.split("=");
            const className = name.substring(6);
            classCookies[className] = value;
        }
    });

    return classCookies;
}

function alternarClasse(elementos, classe) {
    elementos.forEach(function (elementoId) {
        const elemento = document.getElementById(elementoId);
        if (elemento) {
            elemento.classList.toggle(classe);

            if (elemento.classList.contains(classe)) {
                setCookie("class_" + elementoId, "true");
            } else {
                deleteCookie("class_" + elementoId);
            }
        }
    });
}

// Chame esta função ao carregar a página
applyClassesFromCookies();
