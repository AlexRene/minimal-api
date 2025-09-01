namespace MinimalApi.Dominio.Servicos;

public interface ISenhaServico
{
    string HashSenha(string senha);
    bool VerificarSenha(string senha, string hash);
}
