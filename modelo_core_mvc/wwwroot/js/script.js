window.addEventListener("load", () => {
    //Captura altura relativa do menu
    let alturaRelativa = document.querySelector("#colunaSideBar").offsetHeight;
    root.style.setProperty('--Altura-menu', alturaRelativa + "px");
})
let root = document.documentElement;
root.style.scrollBehavior = "auto";
var mobile = false;

removerPalavras();
let menuLateralEstaAberto = false;

// Chamar icones icone e add icones-menu
function openNav() {
    document.getElementById("abrirMenu").classList.add("d-none");
    document.getElementById("fecharMenu").classList.remove("d-none");
    document.getElementById("menuBar").classList.add("menu-lateral-aberto");
    document.getElementById("menuBar").classList.remove("menu-lateral-fechado");
    document.getElementById("fecharMenu").classList.add("btn-direita");
    document.getElementById("ul-raiz").classList.remove("hideMenu");
    addIconsMenu();
    mostrarPalavras();
    document.getElementById("abrirMenuHamburguer").style.display = "none";
    document.getElementById("fecharMenuHamburguer").style.display = "block";
    document.getElementsByClassName("dropBox")[0].classList.remove("dropdown-menu-fechado");
    document.getElementsByClassName("dropBox")[1].classList.remove("dropdown-menu-fechado-componentes");
    document.getElementsByClassName("dropBox")[2].classList.remove("dropdown-menu-fechado");
    document.getElementsByClassName("hr")[0].classList.remove("hr-menu-lateral-fechado");
    document.getElementsByClassName("hr")[0].classList.add("hr-menu-lateral-aberto");
    document.getElementById("navbarSupportedContent").classList.remove("navbar-supported-content-fechado");
    document.getElementById("navbarSupportedContent").classList.add("navbar-supported-content-aberto");
    document.getElementsByClassName("item-menu")[0].classList.remove("icones-menu-fechado-selecionado-tab");
    document.getElementsByClassName("item-menu")[1].classList.remove("icones-menu-fechado-selecionado-tab");
    document.getElementsByClassName("item-menu")[2].classList.remove("icones-menu-fechado-selecionado-tab");
    menuLateralEstaAberto = true;
    document.getElementById("colunaPrincipal").classList.add("conteudo-comprimido")
    document.getElementById("colunaPrincipal").classList.remove("conteudo-expandido")
}

function closeNav() {
    document.getElementById("fecharMenu").classList.add("d-none", "btn-esquerda");
    document.getElementById("abrirMenu").classList.remove("d-none");
    document.getElementById("menuBar").classList.remove("menu-lateral-aberto");
    document.getElementById("ul-raiz").classList.add("hideMenu");
    removerPalavras();
    document.getElementById("fecharMenuHamburguer").style.display = "none";
    document.getElementById("abrirMenuHamburguer").style.display = "block";
    document.getElementsByClassName("item-menu")[0].classList.remove("li-hover", "forcar-focus", "forcar-focus-within", "icones-menu-fechado-selecionado-tab");
    document.getElementsByClassName("item-menu")[1].classList.remove("li-hover", "forcar-focus", "forcar-focus-within-componentes", "icones-menu-fechado-selecionado-tab");
    document.getElementsByClassName("item-menu")[2].classList.remove("li-hover", "forcar-focus", "forcar-focus-within", "icones-menu-fechado-selecionado-tab");
    document.getElementsByClassName("dropBox")[0].classList.add("dropdown-menu-fechado");
    document.getElementsByClassName("dropBox")[0].classList.remove("show");
    document.getElementsByClassName("dropBox")[1].classList.add("dropdown-menu-fechado-componentes");
    document.getElementsByClassName("dropBox")[1].classList.remove("show");
    document.getElementsByClassName("dropBox")[2].classList.add("dropdown-menu-fechado");
    document.getElementsByClassName("dropBox")[2].classList.remove("show");
    document.getElementsByClassName("hr")[0].classList.add("hr-menu-lateral-fechado");
    document.getElementsByClassName("hr")[0].classList.remove("hr-menu-lateral-aberto");
    menuLateralEstaAberto = false
    document.getElementById("colunaPrincipal").classList.remove("conteudo-comprimido")
    document.getElementById("colunaPrincipal").classList.add("conteudo-expandido")
}

document.getElementById('botao-hamburguinho').addEventListener('click', () => {
    openNav()
})
document.getElementById('abrirMenu').addEventListener('click', () => {
    openNav()
})
document.getElementById('botao-hamburguinho').addEventListener('click', () => {
    closeNav()
})
document.getElementById('fecharMenu').addEventListener('click', () => {
    closeNav()
})

// //Adiciona os icones ao menu lateral aberto
function addIconsMenu() {
    let arrayDeIcones = Array.from(document.getElementsByClassName("icone"));
    arrayDeIcones.map(icone => {
        icone.classList.add("icones-menu");
        icone.classList.remove("icones-menu-fechado");
    })
}

function mostrarPalavras() {

    let palavra = document.getElementsByClassName("palavra");
    palavra.item(0).style.display = "block";

    for (let i = 1; i < 7; i++) {
        palavra.item(i).style.display = "inline"
    }

    document.getElementsByClassName("item-menu").item(0).classList.remove("setinha-d-none")
    document.getElementsByClassName("item-menu").item(1).classList.remove("setinha-d-none")
    document.getElementsByClassName("item-menu").item(2).classList.remove("setinha-d-none")

    let arrayItemMenu = Array.from(document.getElementsByClassName("item-menu"));
    arrayItemMenu.map(item => {
        item.classList.add("li-menu-aberto");
    })
}

function removerPalavras() {

    document.getElementById("menuBar").classList.add("menu-lateral-fechado");
    let palavra = document.getElementsByClassName("palavra");
    palavra.item(0).style.display = "none";
    for (let i = 1; i < 7; i++) {
        palavra.item(i).style.display = ""
    }
    document.getElementsByClassName("item-menu").item(0).classList.add("setinha-d-none");
    document.getElementsByClassName("item-menu").item(1).classList.add("setinha-d-none");
    document.getElementsByClassName("item-menu").item(2).classList.add("setinha-d-none");
    removeIconsMenu();
    let arrayItemMenu = Array.from(document.getElementsByClassName("item-menu"));
    arrayItemMenu.map(item => {
        item.classList.remove("li-menu-aberto");
    })
    document.getElementById("fecharMenuHamburguer").style.display = "none";
    document.getElementById("abrirMenuHamburguer").style.display = "block";
    document.getElementById("navbarSupportedContent").classList.add("navbar-supported-content-fechado");
    document.getElementById("navbarSupportedContent").classList.remove("navbar-supported-content-aberto");
}

function removeIconsMenu() {
    let arrayDeIcones = Array.from(document.getElementsByClassName("icone"));
    arrayDeIcones.map(icone => {
        icone.classList.remove("icones-menu");
        icone.classList.add("icones-menu-fechado");
    })
}

if (window.screen.width >= 992) {
    document.getElementsByClassName("item-menu")[0].addEventListener("keydown", (key) => {
        //Rever
        if (key.key == "Enter") {
            if (document.getElementsByClassName('dropBox')[0].classList.contains('show')) {
                document.getElementsByClassName('dropBox')[0].classList.remove('show')
                document.getElementsByClassName('dropBox')[0].removeAttribute('data-bs-popper', 'static')
            } else {
                document.getElementsByClassName('dropBox')[0].classList.add('show')
                document.getElementsByClassName('dropBox')[0].setAttribute('data-bs-popper', 'static')
                document.getElementsByClassName('dropBox')[1].classList.remove('show')
                document.getElementsByClassName('dropBox')[1].removeAttribute('data-bs-popper', 'static')
            }

        }
    });
    document.getElementById("home").addEventListener("focusout", () => {
        //Rever
    })
    document.getElementsByClassName('item-menu')[0].addEventListener("click", () => {
        //Rever
        if (document.getElementById('menuBar').classList.contains('menu-lateral-fechado')) {
            openNav()
            document.getElementsByClassName('dropBox')[0].classList.add('show')
            document.getElementsByClassName('dropBox')[0].setAttribute('data-bs-popper', 'static')
        }
    })
    document.getElementsByClassName('item-menu')[0].addEventListener("focusin", () => {
        //rever
        document.getElementsByClassName('dropBox')[1].classList.remove('show')
        document.getElementsByClassName('dropBox')[1].removeAttribute('data-bs-popper', 'static')
    });
    document.getElementById("home").addEventListener("mouseleave", () => {
        //rever
        if (document.getElementById("menuBar").classList.contains("menu-lateral-fechado")) {

            document.getElementsByClassName('dropBox')[0].classList.remove('show')
            document.getElementsByClassName('dropBox')[0].removeAttribute('data-bs-popper', 'static')
        }

    })
    document.getElementsByClassName('item-menu')[0].addEventListener('mouseenter', () => {
        //rever
        if (document.getElementById('menuBar').classList.contains('menu-lateral-fechado')) {
            document.getElementsByClassName('dropBox')[0].classList.add('show')
            document.getElementsByClassName('dropBox')[0].setAttribute('data-bs-popper', 'static')
        }
    })

    document.getElementsByClassName("item-menu")[1].addEventListener("keydown", (key) => {
        if (key.key == "Enter") {
            if (document.getElementsByClassName('dropBox')[1].classList.contains('show')) {
                document.getElementsByClassName('dropBox')[1].classList.remove('show')
                document.getElementsByClassName('dropBox')[1].removeAttribute('data-bs-popper', 'static')
            } else {
                document.getElementsByClassName('dropBox')[1].classList.add('show')
                document.getElementsByClassName('dropBox')[1].setAttribute('data-bs-popper', 'static')
                document.getElementsByClassName('dropBox')[0].classList.remove('show')
                document.getElementsByClassName('dropBox')[2].classList.remove('show')
                document.getElementsByClassName('dropBox')[0].removeAttribute('data-bs-popper', 'static')
                document.getElementsByClassName('dropBox')[2].removeAttribute('data-bs-popper', 'static')
            }

        }
    });
    document.getElementById("componentes").addEventListener("focusout", () => {
    })
    document.getElementsByClassName('item-menu')[1].addEventListener("click", () => {
        if (document.getElementById('menuBar').classList.contains('menu-lateral-fechado')) {
            openNav()
            document.getElementsByClassName('dropBox')[1].classList.add('show')
            document.getElementsByClassName('dropBox')[1].setAttribute('data-bs-popper', 'static')
        }
    })
    document.getElementsByClassName('item-menu')[1].addEventListener("focusin", () => {
        document.getElementsByClassName('dropBox')[0].classList.remove('show')
        document.getElementsByClassName('dropBox')[2].classList.remove('show')
        document.getElementsByClassName('dropBox')[0].removeAttribute('data-bs-popper', 'static')
        document.getElementsByClassName('dropBox')[2].removeAttribute('data-bs-popper', 'static')
    });
    document.getElementById("componentes").addEventListener("mouseleave", () => {
        if (document.getElementById("menuBar").classList.contains("menu-lateral-fechado")) {
            fixarMenu(1)
            document.getElementsByClassName('dropBox')[1].classList.remove('show')
            document.getElementsByClassName('dropBox')[1].removeAttribute('data-bs-popper', 'static')
        }

    })
    document.getElementsByClassName('dropBox')[1].addEventListener('keydown', (key) => {
        if (key.key == "Tab") {
            //Erro causado aqui
            desfixarMenu(1)
        }
    })
    document.getElementsByClassName('item-menu')[1].addEventListener('mouseenter', () => {
        //Erro causado aqui
        desfixarMenu(1)
        if (document.getElementById('menuBar').classList.contains('menu-lateral-fechado')) {
            document.getElementsByClassName('dropBox')[1].classList.add('show')
            document.getElementsByClassName('dropBox')[1].setAttribute('data-bs-popper', 'static')
        }
    })

    document.getElementsByClassName("item-menu")[2].addEventListener("keydown", (key) => {
        //Rever
        if (key.key == "Enter") {
            if (document.getElementsByClassName('dropBox')[2].classList.contains('show')) {
                document.getElementsByClassName('dropBox')[2].classList.remove('show')
                document.getElementsByClassName('dropBox')[2].removeAttribute('data-bs-popper', 'static')
            } else {
                document.getElementsByClassName('dropBox')[2].classList.add('show')
                document.getElementsByClassName('dropBox')[2].setAttribute('data-bs-popper', 'static')
                document.getElementsByClassName('dropBox')[0].classList.remove('show')
                document.getElementsByClassName('dropBox')[1].classList.remove('show')
                document.getElementsByClassName('dropBox')[0].removeAttribute('data-bs-popper', 'static')
                document.getElementsByClassName('dropBox')[1].removeAttribute('data-bs-popper', 'static')
            }

        }
    });
    document.getElementById("templates").addEventListener("focusout", () => {
        //Rever
    })
    document.getElementsByClassName('item-menu')[2].addEventListener("click", () => {
        //Rever
        if (document.getElementById('menuBar').classList.contains('menu-lateral-fechado')) {
            openNav()
            document.getElementsByClassName('dropBox')[2].classList.add('show')
            document.getElementsByClassName('dropBox')[2].setAttribute('data-bs-popper', 'static')
        }
    })
    document.getElementsByClassName('item-menu')[2].addEventListener("focusin", () => {
        //rever
        document.getElementsByClassName('dropBox')[0].classList.remove('show')
        document.getElementsByClassName('dropBox')[1].classList.remove('show')
        document.getElementsByClassName('dropBox')[0].removeAttribute('data-bs-popper', 'static')
        document.getElementsByClassName('dropBox')[1].removeAttribute('data-bs-popper', 'static')
    });
    document.getElementById("templates").addEventListener("mouseleave", () => {
        //rever
        if (document.getElementById("menuBar").classList.contains("menu-lateral-fechado")) {
            document.getElementsByClassName('dropBox')[2].classList.remove('show')
            document.getElementsByClassName('dropBox')[2].removeAttribute('data-bs-popper', 'static')
        }

    })
    document.getElementsByClassName('item-menu')[2].addEventListener('mouseenter', () => {
        //rever
        if (document.getElementById('menuBar').classList.contains('menu-lateral-fechado')) {
            document.getElementsByClassName('dropBox')[2].classList.add('show')
            document.getElementsByClassName('dropBox')[2].setAttribute('data-bs-popper', 'static')
        }
    })

    //Templates -> vai ter que ser acessibilidade
    document.getElementById("acessibilidade").addEventListener("focusin", () => {
        document.getElementsByClassName('dropBox')[0].classList.remove('show')
        document.getElementsByClassName('dropBox')[0].removeAttribute('data-bs-popper', 'static')
        document.getElementsByClassName('dropBox')[1].classList.remove('show')
        document.getElementsByClassName('dropBox')[1].removeAttribute('data-bs-popper', 'static')
        document.getElementsByClassName('dropBox')[2].classList.remove('show')
        document.getElementsByClassName('dropBox')[2].removeAttribute('data-bs-popper', 'static')
    })
    document.getElementsByClassName('item-menu')[3].addEventListener('focus', () => {
        rmvTodosFocus()
    })
} else {
}

function esconderItens() {
    //rever
    document.getElementsByClassName("item-menu")[0].classList.remove("forcar-focus", "forcar-focus-within", "setinha-baixo");
    document.getElementsByClassName("item-menu")[1].classList.remove("forcar-focus", "forcar-focus-within-componentes", "setinha-baixo");
}

document.getElementById("abrirMenu").addEventListener("focusin", () => {
    esconderItens()
})

document.getElementById("fecharMenu").addEventListener("focusin", () => {
    esconderItens()
})

document.getElementById('search-bar').addEventListener('keydown', (tecla) => {
    if (tecla.key == "Enter") {

    }
})

let aberto = false;

function rmvTodosFocus() {
    fixarMenu(1)
}

function forcarFloatFunction(idInput, idLabel) {
    document.getElementById(idInput).addEventListener('click', () => {
        document.getElementById(idLabel).classList.add('forcar-float');
    })
    document.getElementById(idInput).addEventListener('focusin', () => {
        document.getElementById(idLabel).classList.add('forcar-float');
    })
    document.getElementById(idInput).addEventListener('focusout', () => {
        if (document.getElementById(idInput).value == " ") {
            document.getElementById(idLabel).classList.remove('forcar-float');
        }
    })
}


function desfixarMenu(indexElemento) {
    // if (indexElemento == 1) { document.getElementById("menuBar").classList.add("mover-menu-lateral"); }
    var alturaScrollY = window.scrollY
    // root.style.setProperty('--Altura-pagina', (alturaScrollY*0.525 - 30) + "px");
    switch (valorMudanca) {
        case 0:
            root.style.setProperty('--Altura-pagina', (alturaScrollY - 58) + "px"); //Tá certo
            break;
        case 1:
            root.style.setProperty('--Altura-pagina', (alturaScrollY * 0.90911 - 58) + "px"); //Máximo q consegui chegar
            break;
        case 2:
            root.style.setProperty('--Altura-pagina', (alturaScrollY * 0.76923 - 58) + "px"); // Tá certo
            break;
        case 3:
            root.style.setProperty('--Altura-pagina', (alturaScrollY * 0.66669 - 58) + "px");//Máximo q consegui chegar
            break;
        case 4:
            root.style.setProperty('--Altura-pagina', (alturaScrollY * 0.571429 - 58) + "px"); // Tá certo
            break;
        case 5:
            root.style.setProperty('--Altura-pagina', (alturaScrollY * 0.5263199 - 58) + "px"); // Tá certo
            break;
    }
}

function fixarMenu(indexElemento) {
    if (indexElemento == 1) { document.getElementById("menuBar").classList.remove("mover-menu-lateral"); }
}

//slider
var slider = document.getElementById("customRange1");
var output = document.getElementById("demo");
// Display the default slider value
output.innerHTML = slider.value;

// Update the current slider value (each time you drag the slider handle)
slider.oninput = function () {
    output.innerHTML = this.value;
}