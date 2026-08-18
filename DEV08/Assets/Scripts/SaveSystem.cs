using UnityEngine;

public static class SaveSystem
{
    // Adicionado "static" na declaração do método
    public static void SalvarEstadoEnergia(string nomeEnergia, bool estaAtiva, bool estaBloqueada)
    {
        PlayerPrefs.SetInt($"{nomeEnergia}_Ativa", estaAtiva ? 1 : 0);
        PlayerPrefs.SetInt($"{nomeEnergia}_Bloqueada", estaBloqueada ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool CarregarEstadoAtivo(string nomeEnergia, bool valorPadrao)
    {
        return PlayerPrefs.GetInt($"{nomeEnergia}_Ativa", valorPadrao ? 1 : 0) == 1;
    }

    public static bool CarregarEstadoBloqueado(string nomeEnergia, bool valorPadrao)
    {
        return PlayerPrefs.GetInt($"{nomeEnergia}_Bloqueada", valorPadrao ? 1 : 0) == 1;
    }

    public static void LimparDados()
    {
        PlayerPrefs.DeleteAll();
    }
}