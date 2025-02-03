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

function atualizarAtributosBotaoExpandir() {
    var botao = document.getElementById('botaoExpandir');
    var painelLateral = document.getElementById('painelLateral');
    var estaRecolhido = painelLateral.classList.contains('recolhido');

    botao.title = "Botao para expandir ou recolher o menu (<alt " + (estaRecolhido ? "." : ",") + ">)";
    botao.setAttribute('aria-label', "Botao para expandir ou recolher o menu (<alt " + (estaRecolhido ? "." : ",") + ">)");
    botao.accessKey = estaRecolhido ? "." : ",";
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

document.addEventListener('keydown', function (event) {
    if (event.altKey) {
        switch (event.key) {
            case '1':
                irParaElemento('itensNav');
                break;
            case '2':
                irParaElemento('conteudo');
                break;
        }
    }
});

function alternarPopup() {
    var menuPopup = document.getElementById('menuPopup');
    if (menuPopup) {
        if (!menuPopup.classList.contains('oculto')) {
            menuPopup.classList.add('oculto');
        }
        else {
            menuPopup.classList.remove('oculto');
        }
        menuPopup.focus();
    }
};

function irParaElemento(id) {
    var elementos = document.querySelectorAll(`#${id}`);
    if (elementos) {
        const elementoVisivel = Array.from(elementos).find(el => el.offsetParent !== null);
        if (elementoVisivel) {
            elementoVisivel.focus();
            const painelPrincipal = document.getElementById('painelPrincipal');
            if (painelPrincipal) {
                painelPrincipal.scrollTo({
                    top: 0,
                    behavior: 'smooth'
                });
            }
        }
        else {
            console.error(`Elemento ${id} nao esta visivel.`);
        }
    }
    else {
        console.error(`Elemento ${id} nao encontrado.`);
    }
}

document.addEventListener('DOMContentLoaded', function () {
    _aguarde(false);

    const painelPrincipal = document.getElementById('painelPrincipal');
    const botaoTopo = document.getElementById('topoDaPagina');
    if (painelPrincipal && botaoTopo) {
        painelPrincipal.addEventListener('scroll', function () {
            if (painelPrincipal.scrollTop > 20) {
                botaoTopo.style.display = 'block';
            } else {
                botaoTopo.style.display = 'none';
            }
        });

        botaoTopo.addEventListener('click', function (e) {
            e.preventDefault();
            painelPrincipal.scrollTo({
                top: 0,
                behavior: 'smooth'
            });
        });
    }
    irParaElemento('conteudo');
});

window.addEventListener('load', function () {
    _selecionar();
});
