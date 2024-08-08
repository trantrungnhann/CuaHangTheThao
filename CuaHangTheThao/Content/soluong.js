function updateQuantity(isan_pham_id, change) {
    // Gọi AJAX để cập nhật số lượng trong giỏ hàng
    $.ajax({
        url: '@Url.Action("SuaMatHang", "DatHang")',
        type: 'POST',
        data: {
            msp: isan_pham_id,
            quantityChange: change
        },
        success: function (result) {
            // Nếu cập nhật thành công, reload lại trang để hiển thị số lượng mới
            location.reload();
        },
        error: function () {
            alert('Cập nhật số lượng không thành công.');
        }
    });
}
