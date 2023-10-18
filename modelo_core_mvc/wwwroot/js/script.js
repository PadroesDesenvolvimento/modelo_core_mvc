function alternarClasse(elementos, classe) {
    elementos.forEach(function (elementoId) {
        const elemento = document.getElementById(elementoId);
        if (elemento) {
            elemento.classList.toggle(classe);
            if (classe === 'recolhido') {
                elemento.setAttribute('aria-expanded', elemento.classList.contains(classe));
            }
        }
    });
}

