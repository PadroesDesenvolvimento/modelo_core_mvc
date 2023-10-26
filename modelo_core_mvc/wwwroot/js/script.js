// Incluir ou excluir uma classe em uma lista de elementos do DOM
function alternarClasse(elementos, classe) {
    elementos.forEach(function (elementoId) {
        const elemento = document.getElementById(elementoId);
        if (elemento) {
            var contemClasse = elemento.classList.contains(classe);
            elemento.classList.toggle(classe);

            // Quando a classe for recolhido, adiciona-se essa informacao para acessibilidade
            if (classe === 'recolhido') {
                elemento.setAttribute('aria-expanded', elemento.classList.contains(classe));
            }
        }
    });
}
