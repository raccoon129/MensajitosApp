using DAL;

namespace MensajeriaWebAPI
{
    public static class Parametros
    {

#if DEBUG
        public static string CadenaDeConexion = @"Server=localhost;Database=mensajeria;Uid=root;Pwd=;";
        public static TipoDB TipoDB = TipoDB.MySQL;
#else

#endif
        public static FabricRepository FabricaRepository = new FabricRepository(CadenaDeConexion, TipoDB);

    }
}
