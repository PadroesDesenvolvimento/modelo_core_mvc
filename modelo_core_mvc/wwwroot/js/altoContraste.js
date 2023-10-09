(function () {
    var Contrast = {
        storage: 'contrastState',
        cssClass: 'alto-contrast',
        currentState: null,
        check: checkContrast,
        getState: getContrastState,
        setState: setContrastState,
        toogle: toogleContrast,
        updateView: updateViewContrast
        
    };
  
    window.toggleContrast = function () { Contrast.toogle(); };

    window.tog
  
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
  
    // function toogleContrast() {
    //     this.setState(!this.currentState);
    // }
  })();

  document.getElementsByClassName('btn-acessibilidade')[2].addEventListener('click', () => {
    console.log("escuro/claro")
    window.toggleContrast()
})

