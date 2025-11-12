window.bootstrapInterop = {
    // Função para mostrar o modal
    showModal: function (modalSelector) {
        // Usa a API do Bootstrap 5 para criar e mostrar o modal
        var modalElement = document.querySelector(modalSelector);
        if (modalElement) {
            var modal = new bootstrap.Modal(modalElement);
            modal.show();
        } else {
            console.error('Elemento modal não encontrado para o seletor:', modalSelector);
        }
    },

    // Função para esconder o modal
    hideModal: function (modalSelector) {
        // Encontra a instância do modal e a esconde
        var modalElement = document.querySelector(modalSelector);
        if (modalElement) {
            var modalInstance = bootstrap.Modal.getInstance(modalElement);
            if (modalInstance) {
                modalInstance.hide();
            } else {
                // Se não houver uma instância, force a remoção das classes de exibição
                modalElement.classList.remove('show');
                modalElement.setAttribute('aria-hidden', 'true');
                modalElement.style.display = 'none';
                document.body.classList.remove('modal-open');
                // Alternativamente, recrie/re-selecione a instância e chame .hide()
            }
        }
    }
};