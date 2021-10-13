
function set_dados_forms(dados) {
    $('#id_cadastro').val(dados.Id);
    $('#txt_nome').val(dados.Nome);
    $('#txt_usuarioProprietario').val(dados.IdUsuario);
    $('#txt_dataCriacao').val(dados.DataCriacao);
    $('#cbx_ativo').prop('checked', dados.Ativo);
}

function set_focus_form() {
    $('#txt_nome').focus();
}

function set_dados_grid(dados) {
    return
        '<td>' + dados.Nome + '</td>' +
        '<td>' + dados.IdUsuario + '</td>' +
        '<td>' + dados.DataCriacao + '</td>' +
        '<td>' + (dados.Ativo ? 'SIM' : 'NÃO') + '</td>';
}

function get_dados_inclusao() {
    return {
        Id: 0,
        Nome: '',
        IdUsuario: 0,
        DataCriacao: '',
        Ativo: true
    }
}

function get_dados_form() {
    return {
        Id: $('#id_cadastro').val(),
        Nome: $('#txt_nome').val(),
        IdUsuario: $('#txt_usuarioProprietario').val(),
        DataCriacao: $('#txt_dataCriacao').val(),
        Ativo: $('#cbx_ativo').prop('checked')
    };
}

function preencher_linha_grid(param, linha) {
    linha
        .eq(0).html(param.Nome).end()
        .eq(1).html(param.IdUsuario).end()
        .eq(2).html(param.DataCriacao).end()
        .eq(3).html(param.Ativo ? 'SIM' : 'NÃO');
}


