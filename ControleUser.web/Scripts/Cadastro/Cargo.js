
function set_dados_forms(dados) {
    $('#id_cadastro').val(dados.Id);
    $('#txt_cargo').val(dados.Cargo);
}

function set_focus_form() {
    $('#txt_cargo').focus();
}

function set_dados_grid(dados) {
    return
    '<td>' + dados.Cargo + '</td>';
}

function get_dados_inclusao() {
    return {
        Id: 0,
        Cargo: ''
    }
}

function get_dados_form() {
    return {
        Id: $('#id_cadastro').val(),
        Cargo: $('#txt_cargo').val()
    };
}

function preencher_linha_grid(param, linha) {
    linha
        .eq(0).html(param.Cargo).end();
}


