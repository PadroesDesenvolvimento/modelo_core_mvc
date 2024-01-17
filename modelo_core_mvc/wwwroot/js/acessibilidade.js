let valorMudanca = 0;
let valores = [1, 1.1, 1.3, 1.5, 1.75, 1.9]
let desconto = [14.5, 19.5, 27.5, 33.5, 38.8, 41.3]

function alterarZoom() {
    if (valorMudanca < 0) { valorMudanca = 0 }
    else if (valorMudanca > 5) { valorMudanca = 5 }
    document.cookie = "valorMudanca=" + valorMudanca + "; path=/";
    document.getElementsByTagName("body").item(0).style.setProperty('zoom', valores[valorMudanca]);
    document.documentElement.style.setProperty('--desconto', desconto[valorMudanca] + 'em');

    atualizarBotoesZoom()
}

function aumentarCaracter() {
    valorMudanca++;
    alterarZoom();
}

function diminuirCaracter() {
    valorMudanca--;
    alterarZoom();
}

function atualizarBotoesZoom() {
    const buttons = document.querySelectorAll('.botaoZoom');
    buttons.forEach(function (button) {
        const ariaLabel = button.getAttribute('aria-label');
        const zoomValue = Math.round(valores[valorMudanca] * 100);
        button.title = `${ariaLabel} - ${zoomValue}%`;
    });
};

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
        toggle: toggleContrast,
        updateView: updateViewContrast
    };

    window.toggleContrast = function () { Contrast.toggle(); };

    Contrast.check();

    function checkContrast() {
        this.updateView();
    }

    function getContrastState() {
        return localStorage.getItem(this.storage) === 'true';
    }

    function setContrastState(state) {
        localStorage.setItem(this.storage, '' + state);
        document.cookie = "contrastState=" + state + "; path=/";
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

    function toggleContrast() {
        this.setState(!this.currentState);
    }
})();

window.onload = function () {
    var body = document.getElementsByTagName("body")[0];
    var valorZoom = window.getComputedStyle(body).getPropertyValue('zoom');
    valorMudanca = valores.indexOf(parseFloat(valorZoom));
    atualizarBotoesZoom();
}
