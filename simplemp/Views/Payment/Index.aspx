<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>
        Create payment</h1>
    <table id="paymentsTable">
        <tr>
            <td>
                Payment Amount
                <td>
                    <input type="text" class="text" id="amount" value="<%=ViewData["amount"] %>">
        </tr>
        <tr>
            <td>
                Payer Fee
            </td>
            <td>
                <input type="text" class="text" id="payer_fee_amount" value="<%=ViewData["payer_fee_amount"] %>">
        </tr>
        <tr>
            <td>
                Recipient Fee
            </td>
            <td>
                <input type="text" class="text" id="recipient_fee_amount" value="<%=ViewData["recipient_fee_amount"] %>">
        </tr>
        <tr>
            <td>
                Payer Email
            </td>
            <td>
                <input type="text" class="text" id="payer_email_address" value="<%=ViewData["payer_email_address"] %>">
        </tr>
        <tr>
            <td>
                Recipient Email
            </td>
            <td>
                <input type="text" class="text" id="recipient_email_address" value="<%=ViewData["recipient_email_address"] %>">
        </tr>
        <tr>
            <td>
                Description
            </td>
            <td>
                <input type="text" class="text" id="description" value="<%=ViewData["description"] %>">
        </tr>
        <tr>
            <td>
                Developer identifier
            </td>
            <td>
                <input type="text" class="text" id="developer_identifier" value="<%=ViewData["developer_identifier"] %>">
        </tr>
    </table>
    <a href="javascript:;" onclick="createPayment();">Create Payment</a>
    <h1>
        Display iframe</h1>
    <table>
        <tr>
            <td>
                Payment request id
                <td>
                    <input type="text" class="text" id="payment_id">
        </tr>
        <tr>
            <td>
                Card holder name
            </td>
            <td>
                <input type="text" class="text" id="cardholder_name">
        </tr>
        <tr>
            <td>
                server
                <td>
                    <input type="text" class="text" id="server" value="<%=ViewData["iframe_root_uri"] %>">
        </tr>
    </table>
    <a href="javascript:;" onclick="startIFrame();">Start Payment IFrame</a> &nbsp;
    <a href="javascript:;" onclick="launchLightbox();">Launch Lightbox</a>
    <div id="pound-root">
    </div>
    <h1 id="paymentComplete" style="display: none; color: green;">
        Payment Complete
    </h1>
    <h1>
        Payment Operations</h1>
    <table>
        <tr>
            <td>
                Payment SID
                <td>
                    <input type="text" class="text" id="operating_payment_sid">
        </tr>
    </table>
    <a href="javascript:;" onclick="escrowPayment();">Escrow Payment</a><br>
    <a href="javascript:;" onclick="releasePayment();">Release Payment</a>
    <pre id="operation_results"></pre>
</asp:Content>
