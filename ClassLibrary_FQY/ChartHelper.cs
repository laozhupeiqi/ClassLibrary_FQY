using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.DataVisualization.Charting;


namespace ClassLibrary_FQY
{
    /// <summary>
    /// Chart控件初始化类
    /// </summary>
    public class ChartHelper
    {
        /// <summary>
        /// 添加序列
        /// </summary>
        /// <param name="chart">图表对象</param>
        /// <param name="seriesName">序列名称</param>
        /// <param name="chartType">图表类型</param>
        /// <param name="color">颜色</param>
        /// <param name="markColor">标记点颜色</param>
        /// <param name="showValue">是否显示数值</param>
        public static void AddSeries(Chart chart, string seriesName, SeriesChartType chartType, Color color, Color markColor, bool showValue = false)
        {
            chart.Series.Add(seriesName);
            chart.Series[seriesName].ChartType = chartType;
            chart.Series[seriesName].Color = color;
            if (showValue)
            {
                chart.Series[seriesName].IsValueShownAsLabel = true;
                chart.Series[seriesName].MarkerStyle = MarkerStyle.Circle;
                chart.Series[seriesName].MarkerColor = markColor;
                chart.Series[seriesName].LabelForeColor = color;
                chart.Series[seriesName].LabelAngle = -90;
            }
        }
        /// <summary>
        /// 设置标题
        /// </summary>
        /// <param name="chart">图表对象</param>
        /// <param name="chartName">图表名称</param>
        /// <param name="font">字体</param>
        /// <param name="docking">停靠位置</param>
        /// <param name="foreColor">字体颜色</param>
        public static void SetTitle(Chart chart, string chartName, Font font, Docking docking, Color foreColor)
        {
            chart.Titles.Clear();
            chart.Titles.Add(chartName);
            chart.Titles[0].Font = font;
            chart.Titles[0].Docking = docking;
            chart.Titles[0].ForeColor = foreColor;
        }
        /// <summary>
        /// 设置样式
        /// </summary>
        /// <param name="chart">图表对象</param>
        /// <param name="backColor">背景颜色</param>
        /// <param name="foreColor">字体颜色</param>
        public static void SetStyle(Chart chart, Color backColor, Color foreColor)
        {
            chart.BackColor = backColor;
            chart.ChartAreas[0].BackColor = backColor;
            chart.ForeColor = foreColor;
        }
        /// <summary>
        /// 设置图例
        /// </summary>
        /// <param name="chart">图表对象</param>
        /// <param name="docking">停靠位置</param>
        /// <param name="align">字符串对齐方式</param>
        /// <param name="backColor">背景颜色</param>
        /// <param name="foreColor">字体颜色</param>
        public static void SetLegend(Chart chart, Docking docking, StringAlignment align, Color backColor, Color foreColor)
        {
            chart.Legends[0].Docking = docking;
            chart.Legends[0].Alignment = align;
            chart.Legends[0].BackColor = backColor;
            chart.Legends[0].ForeColor = foreColor;
            
        }
        /// <summary>
        /// 图表区 设置XY轴
        /// </summary>
        /// <param name="chart">图表对象</param>
        /// <param name="xTitle">X轴标题</param>
        /// <param name="yTitle">Y轴标题</param>
        /// <param name="align">坐标轴标题对齐方式</param>
        /// <param name="foreColor">坐标轴字体颜色</param>
        /// <param name="lineColor">坐标轴颜色</param>
        /// <param name="arrowStyle">坐标轴箭头样式</param>
        /// <param name="xInterval">X轴的间距</param>
        /// <param name="yInterval">Y轴的间距</param>
        public static void SetXY(Chart chart, string xTitle, string yTitle, string Format, DateTimeIntervalType Intervaltype, double Interval,bool enable, double Minimum, double Maximum)
        {
            chart.ChartAreas[0].AxisX.Title = xTitle;
            chart.ChartAreas[0].AxisY.Title = yTitle;
            chart.ChartAreas[0].AxisX.LabelStyle.Format = Format;
            chart.ChartAreas[0].AxisX.LabelStyle.IntervalType = Intervaltype;
            chart.ChartAreas[0].AxisX.LabelStyle.Interval = Interval;
            chart.ChartAreas[0].AxisX.LabelStyle.IsEndLabelVisible = enable;
            chart.ChartAreas[0].AxisX.Minimum = Minimum;
            chart.ChartAreas[0].AxisY.Maximum = Maximum;
        }
        /// <summary>
        ///  图表区设置网格
        /// </summary>
        /// <param name="chart">图表对象</param>
        /// <param name="Intervaltype">网格线间隔类型</param>
        /// <param name="xInterval">X轴网格的间距</param>
        public static void SetMajorGrid(Chart chart, DateTimeIntervalType Intervaltype, double xInterval)
        {
            chart.ChartAreas[0].AxisX.MajorGrid.IntervalType = Intervaltype;
            chart.ChartAreas[0].AxisX.MajorGrid.Interval = xInterval;
        }
        /// <summary>
        /// 图表区设置X轴滚动轴
        /// </summary>
        /// <param name="chart">图表对象</param>
        /// <param name="IsPositionedInside">指示滚动条处于内部或外部图表区</param>
        public static void SetScrollBar(Chart chart,bool IsPositionedInside)
        {
            chart.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = IsPositionedInside;
        }
        /// <summary>
        /// 图表区设置显示区域
        /// </summary>
        /// <param name="chart">图表对象</param>
        /// <param name="enable">启用/禁用用户界面缩放</param>
        public static void SetScaleView(Chart chart, bool enable)
        {
            chart.ChartAreas[0].AxisX.ScaleView.Zoomable = enable;
        }

        /// <summary>
        /// 图表区X轴游标设置
        /// </summary>
        /// <param name="chart">图表对象</param>
        /// <param name="enable">启用/禁用游标用户界面和图表区范围选择用户界面</param>
        /// <param name="Interval">游标移动间隔</param>
        /// <param name="Intervaltype">游标移动间隔类型</param>
        public static void SetCursorX(Chart chart,bool enable,int Interval, DateTimeIntervalType Intervaltype)
        {
            chart.ChartAreas[0].CursorX.IsUserEnabled = enable;
            chart.ChartAreas[0].CursorX.IsUserSelectionEnabled = enable;
            chart.ChartAreas[0].CursorX.Interval = Interval;
            chart.ChartAreas[0].CursorX.IntervalOffset = Interval;
            chart.ChartAreas[0].CursorX.IntervalType = Intervaltype;

        }
    }
}
