let valorMudanca = 0;
let valores = [1, 1.1, 1.3, 1.5, 1.75, 1.9]
let desconto = [13, 18, 26, 32, 37.3, 39.8]
let contrastState = "false";

function alterarZoom() {
    if (valorMudanca < 0) { valorMudanca = 0 }
    else if (valorMudanca > 5) { valorMudanca = 5 }
    document.cookie = "valorMudanca=" + valorMudanca + "; path=/";
    document.documentElement.style.setProperty('--desconto', desconto[valorMudanca] + 'em');
    document.getElementsByTagName("body").item(0).style.setProperty('zoom', valores[valorMudanca]);

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
        var cookies = document.cookie.split('; ');
        for (var i = 0; i < cookies.length; i++) {
            var cookie = cookies[i].split('=');

            if (cookie[0] === 'contrastState') {
                return cookie[1] === 'true';
            }
        }
        return null;
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
