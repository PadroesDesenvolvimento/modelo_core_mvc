function mostrarAguarde() {
    const loadingModal = document.getElementById("loading-modal");
    loadingModal.style.display = "block";
}

function ocultarLoadingModal() {
    const loadingModal = document.getElementById("loading-modal");
    loadingModal.style.display = "none";
}

window.addEventListener("beforeunload", setTimeout(mostrarAguarde, 1000));

window.addEventListener("load", ocultarLoadingModal);
