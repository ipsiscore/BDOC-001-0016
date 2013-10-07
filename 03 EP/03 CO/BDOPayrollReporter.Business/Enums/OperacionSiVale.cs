namespace BDOPayrollReporter.Business.Enums
{
    /// <summary>
    /// Enumeración que define los meses.
    /// </summary>
    public enum OperacionSiVale : int
    {
        [StringValue("Mis Compras Solicitud Tarjeta")]
        MisComprasSolicitud = 0,

        [StringValue("Mis Compras Solicitud Adicionales")]
        MisComprasSolicitudAdicionales = 1,

        [StringValue("Mis Compras Alimentacion Solicitud")]
        MisComprasAlimentacionSolicitud = 2,

        [StringValue("Mis Compras Alimentacion Solicitud Adicionales")]
        MisComprasAlimentacionSolicitudAdicionales = 3,

        [StringValue("Mis Compras Despensa Saldo")]
        MisComprasDespensaSaldo = 4,

        [StringValue("Mis Compras Comida Saldo")]
        MisComprasComidaSaldo = 5,

        [StringValue("Mis Compras Alimentacion Saldo")]
        MisComprasAlimentacionSaldo = 6
    }
}