function alternarOculto(elemento) {
    const menu = document.getElementById(elemento);
    if (menu.classList.contains('oculto')) {
        menu.classList.remove('oculto');
        menu.classList.add('visivel');
    } else {
        if (menu.classList.contains('visivel')) {
            menu.classList.remove('visivel');
        }
        menu.classList.add('oculto');
    }
}

function alternarRecolhido(elemento) {
    const menu = document.getElementById(elemento);
    if (menu.classList.contains('recolhido')) {
        menu.classList.remove('recolhido');
        menu.classList.add('expandido');
    } else {
        if (menu.classList.contains('expandido')) {
            menu.classList.remove('expandido');
        }
        menu.classList.add('recolhido');
    }
}