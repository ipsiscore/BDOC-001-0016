<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Menu.ascx.cs" Inherits="BDOPayrollReporter.Menu" %>
<ul>
    <li id="reports" class="expanded"><a href="javascript:void('0');">Reportes</a>
        <ul>
            <li class="leaf"><a href='<%# ResolveUrl("~/Reportes/ReporteReciboNomina.aspx")%>'>Recibo
                de Nómina</a> </li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Reportes/ReporteEmpleadosPorConcepto.aspx")%>'>
                Empleados por Concepto</a></li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Reportes/ReporteAcumuladoConceptosXEmpleado.aspx")%>'>
                Acumulados de Conceptos por Empleado</a></li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Reportes/ReporteResumenConceptosPorCentroCostos.aspx")%>'>
                Resumen de Conceptos por Centro de Costo</a> </li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Reportes/ReporteResumenConceptosPorEmpresa.aspx")%>'>
                Resumen de Conceptos por Empresa</a> </li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Reportes/ReporteAcumuladoConceptosXRegistroPatronal.aspx?tipo=1")%>'>
                Acumulado de conceptos total</a></li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Reportes/ReporteAcumuladoConceptosXRegistroPatronal.aspx?tipo=2")%>'>
                Acumulado de conceptos por período</a></li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Reportes/ReporteImpuestoNomina.aspx")%>'>
                Reporte Impuesto Sobre la Nómina</a></li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Reportes/ReporteImportesGravadosExcentos.aspx")%>'>
                Listado de Importes Gravados y Exentos</a></li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Reportes/ReporteListadoDeValesDeDespensaCentroDeCostos.aspx")%>'>
                Listado de Vales de Despensa (Centro de Costos)</a> </li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Reportes/ReporteListadoDeValesDeDespensa.aspx")%>'>
                Listado de Vales de Despensa</a> </li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Reportes/ReportAportacionesFondoDeAhorro.aspx")%>'>
                Reporte de Aportaciones Fondo de Ahorro</a></li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Reportes/ReporteFondoDeAhorro.aspx")%>'>
                Reporte Fondo de Ahorro</a></li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Reportes/ReportePlanDePensiones.aspx")%>'>
                Reporte Plan de Pensiones</a></li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Reportes/ReporteAportacionesPlanPensiones.aspx")%>'>
                Reporte de Aportaciones Plan de Pensiones</a></li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Reportes/ReporteMaestroDeEmpleados.aspx")%>'>
                Maestro de Empleados</a> </li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Reportes/ReporteControlCambioNomina.aspx")%>'>
                Control de Cambio en Nómina</a></li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Reportes/ReporteProvisionesCentroCosto.aspx")%>'>
                Provisiones por Centro de Costo</a></li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Reportes/ReporteTransferenciaBMX.aspx")%>'>
                Transferencia BANAMEX</a></li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Reportes/WebFormListadoNomina.aspx")%>'>
                Listado de Nómina</a></li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Reportes/ReporteDiferenciasNomina.aspx")%>'>
                Diferencias de Nómina</a></li>
        </ul>
    </li>
    <li id="layouts" class="expanded"><a href="javascript:void('0');">Layout</a>
        <ul>
            <li class="leaf"><a href='<%# ResolveUrl("~/Layouts/LayoutCityBank484.aspx")%>'>Layout
                de CityBank 484</a> </li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Layouts/LayoutCityBank485.aspx")%>'>Layout
                de CityBank 485</a> </li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Layouts/LayoutBanorteAlta.aspx")%>'>Layout
                de altas de Banorte</a> </li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Layouts/LayoutBanorte.aspx")%>'>Layout de
                Banorte</a> </li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Layouts/LayoutSkandiaAlta.aspx")%>'>Layout
                de altas Skandia</a> </li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Layouts/LayoutSkandiaAportaciones.aspx")%>'>
                Layout de aportaciones Skandia</a></li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Layouts/LayoutSkandiaBajas.aspx")%>'>Layout
                de bajas Skandia</a> </li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Layouts/LayoutInboundSap.aspx")%>'>Layout
                importación a SAP</a> </li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Layouts/LayoutSivaleOperaciones.aspx")%>'>
                Layout SiVale</a> </li>
        </ul>
    </li>
    <li id="anteriores" class="expanded"><a href="javascript:void('0');">Anteriores</a>
        <ul>
            <li class="leaf"><a href='<%# ResolveUrl("~/Layouts/LayoutSiValeAlta.aspx")%>'>Layout
                de SíVale Solicitud de Tarjetas</a></li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Layouts/LayoutSiVale.aspx")%>'>Layout de
                SíVale Carga de Saldos</a></li>
            <li class="leaf"><a href='<%# ResolveUrl("~/Layouts/LayoutTepca.aspx") %>'>Layout Tepca</a></li>
        </ul>
    </li>
</ul>