function set_dados_forms(dados) {
    $('#id_cadastro').val(dados.Id);
    $('#txt_nome').val(dados.Nome);
    $('#txt_email').val(dados.Email);
    $('#txt_login').val(dados.Login);
    $('#ddl_cargo').val(dados.IdCargo);
    $('#txt_senha').val(dados.Senha);
    $('#ddl_perfil').val(dados.IdPerfil);
    $('#cbx_ativo').prop('checked', dados.Ativo);
}

function set_focus_form() {
    $('#txt_nome').focus();
}

function set_dados_grid(dados) {
    return
        '<td>' + dados.Nome + '</td>' +
        '<td>' + dados.Email + '</td>' +
        '<td>' + dados.Login + '</td>' +
        '<td>' + (dados.Ativo ? 'SIM' : 'NÃO') + '</td>';
}

function get_dados_inclusao() {
    return {
        Id: 0,
        Nome: '',
        Email: '',
        Login: '',
        IdCargo: 0,
        Senha: '',
        IdPerfil: 0,
        Ativo: true
    };
}

function get_dados_form() {
    return {
        Id: $('#id_cadastro').val(),
        Nome: $('#txt_nome').val(),
        Email: $('#txt_email').val(),
        Login: $('#txt_login').val(),
        IdCargo: $('#ddl_cargo').val(),
        Senha: $('#txt_senha').val(),
        IdPerfil: $('#ddl_perfil').val(),
        Ativo: $('#cbx_ativo').prop('checked')
    };
}

function preencher_linha_grid(param, linha) {
    linha
        .eq(0).html(param.Nome).end()
        .eq(1).html(param.Email).end()
        .eq(2).html(param.Login).end()
        .eq(3).html(param.Ativo ? 'SIM' : 'NÃO');
}


