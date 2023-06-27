let valorMudanca = 0;

// ***************************************************************************************************************************************//
//                                                      Aumentar caractere                                                                //
// ***************************************************************************************************************************************//

function aumentarCaracter(){
  let valores = [1,1.1,1.3,1.5,1.75,1.9]
  let alturaMenuLateral = ['0','0','1px','-125px','-241px']
  valorMudanca = valorMudanca + 1;
  if(valorMudanca < 0) {valorMudanca = 0}
  else if(valorMudanca > 5) {valorMudanca = 5}
  document.getElementsByTagName("body").item(0).style.setProperty('zoom', valores[valorMudanca])
  document.getElementById("ul-raiz").style.setProperty('margin-top', alturaMenuLateral[valorMudanca])
}

// ***************************************************************************************************************************************//
//                                                      Dimiuir caractere                                                                 //
// ***************************************************************************************************************************************//

function diminuirCaracter(){
  let valores = [1,1.1,1.3,1.5,1.75,1.9]
  let alturaMenuLateral = ['0','0','1px','-125px','-241px']
  valorMudanca = valorMudanca - 1;
  if(valorMudanca < 0) {valorMudanca = 0}
  else if(valorMudanca > 5) {valorMudanca = 5}
  document.getElementsByTagName("body").item(0).style.setProperty('zoom', valores[valorMudanca])
  document.getElementById("ul-raiz").style.setProperty('margin-top', alturaMenuLateral[valorMudanca])
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

// ***************************************************************************************************************************************//
//                                                          Event listeners                                                               //
// ***************************************************************************************************************************************//

document.getElementsByClassName('btn-acessibilidade')[0].addEventListener('click', () => {
    aumentarCaracter()
})
document.getElementsByClassName('btn-acessibilidade')[1].addEventListener('click', () => {
    diminuirCaracter()
})
document.getElementsByClassName('btn-acessibilidade')[2].addEventListener('click', () => {
    window.toggleContrast()
})