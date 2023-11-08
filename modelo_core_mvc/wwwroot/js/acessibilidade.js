let valorMudanca = 0;
let valores = [1, 1.1, 1.3, 1.5, 1.75, 1.9]
function atualizarBotoesZoom() {
    const buttons = document.querySelectorAll('.botaoZoom');
    buttons.forEach(function (button) {
        const ariaLabel = button.getAttribute('aria-label');
        const zoomValue = Math.round(valores[valorMudanca] * 100);
        button.title = `${ariaLabel} - ${zoomValue}%`;
    });
};

function aumentarCaracter() {
    valorMudanca = valorMudanca + 1;
    if (valorMudanca < 0) { valorMudanca = 0 }
    else if (valorMudanca > 5) { valorMudanca = 5 }
    document.cookie = "valorMudanca=" + valorMudanca + "; path=/";
    document.getElementsByTagName("body").item(0).style.setProperty('zoom', valores[valorMudanca]);
    atualizarBotoesZoom()
}

function diminuirCaracter() {
    valorMudanca = valorMudanca - 1;
    if (valorMudanca < 0) { valorMudanca = 0 }
    else if (valorMudanca > 5) { valorMudanca = 5 }
    document.cookie = "valorMudanca=" + valorMudanca + "; path=/";
    document.getElementsByTagName("body").item(0).style.setProperty('zoom', valores[valorMudanca]);
    atualizarBotoesZoom()
}


// ***************************************************************************************************************************************//
//                                                        Alto contrste                                                                   //
// ***************************************************************************************************************************************//
(function () {
    var Contrast = {
        storage: 'contrastState',
        cssClass: 'color-blind',
        currentState: null,
        check: checkContrast,
        getState: getContrastState,
        setState: setContrastState,
        toogle: toogleContrast,
        updateView: updateViewContrast
    };

    window.toggleContrast = function () { Contrast.toogle(); };

    Contrast.check();

    function checkContrast() {
        this.updateView();
    }

    function getContrastState() {
        return localStorage.getItem(this.storage) === 'true';
    }

    function setContrastState(state) {
        localStorage.setItem(this.storage, '' + state);
        this.currentState = state;
        this.updateView();
    }

    function updateViewContrast() {
        var body = document.body;

        if (!body) return;

        if (this.currentState === null)
            this.currentState = this.getState();

        if (this.currentState)
            body.classList.add(this.cssClass);
        else
            body.classList.remove(this.cssClass);
    }

    function toogleContrast() {
        this.setState(!this.currentState);
    }
})();

window.onload = function () {
    var body = document.getElementsByTagName("body")[0];
    var valorZoom = window.getComputedStyle(body).getPropertyValue('zoom');
    valorMudanca = valores.indexOf(parseFloat(valorZoom));
    atualizarBotoesZoom()
}
