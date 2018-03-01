$(function () {
    var commonSelectBox = function (order) {
        var select = '<select id="column' + order + '" class="form-control">';
        $.each(csvColumns, function (index, value) {
            select += '<option value="' + value + '">' + value + '</option>'
        });
        select += '</select>';
        return select;
    }

    var showModalMessage = function (message, isError) {
        if (isError) {
            $('#modal-message').removeClass('alert-success');
            $('#modal-message').addClass('alert-danger');
        } else {
            $('#modal-message').addClass('alert-success');
            $('#modal-message').removeClass('alert-danger');
        }
        $('#modal-message').text(message);
        $('#modal-show-message').modal();
    }

    var csvColumns = ['NoMapped', 'SKU', 'Brand', 'Price', 'Weight', 'Feature', 'Product parameter', 'Ignore'];

    var loader = $.connection.loaderHub;
    loader.client.sendProgress = function (progressPercentage) {
        $('.progress').removeClass('hidden');
        var value = progressPercentage + '%';
        $('.progress-bar').css('width', value);
        $('.progress-text').text(value);
    };

    loader.client.sendMessage = function (message, isError) {
        if (isError) {
            $('.progress').addClass('hidden');
            $('#loadFile').removeClass('hidden');
        } else {
            var value = '100%';
            $('.progress-bar').css('width', value);
            $('.progress-text').text(value);
        }        
        $('#file').prop('disabled', false);
        showModalMessage(message, isError);
    };

    var id, fields;
    var columns = [];

    $.connection.hub.start().done(function () {
        id = $.connection.hub.id;

        $('#file').change(function (e) {
            var fileData = $(e.target).get(0).files;
            var file = fileData[0];
            if (file.name.split('.').pop() != 'csv') {
                showModalMessage('Вы загрузили некорректный файл. Загрузите файл с расширением CSV', true);
                return;
            }
            var fr = new FileReader();
            fr.readAsText(file);
            fr.onload = (function (e) {
                $('table.table tbody').empty();
                var lines = e.target.result.split('\r\n');
                columns = lines[0].split(',');
                for (var i = 0; i < columns.length; i++) {
                    $('table.table tbody').append('<tr><td>' + columns[i] + '</td><td>' + commonSelectBox(i) + '</td></tr>');
                    $('#column' + i).val(csvColumns[0]);
                }
                $('table.table').removeClass('hidden');
                $('.progress').addClass('hidden');
                $('#loadFile').removeClass('disabled hidden');
            });
        });

        $('#loadFile').click(function () {
            var fileData = $('#file').get(0).files;
            if (fileData.length != 1) {
                return;
            }
            if (columns.length == 0) {
                showModalMessage('Вы загрузили пустой файл.', true);
                return;
            }
            fields = [];
            for (var i = 0; i < columns.length; i++) {
                fields.push($('#column' + i).val());
            }
            var file = fileData[0];
            var fr = new FileReader();
            fr.readAsDataURL(file);
            fr.onload = (function (e) {
                var data = new FormData();
                data.append('connectionId', id);
                data.append('base64File', e.target.result.substr(e.target.result.indexOf(',') + 1));
                var arr = new Array();
                for (var i = 0; i < fields.length; i++) {
                    data.append('fields', fields[i]);
                }
                $('#loadFile').addClass('hidden');
                $('#file').prop('disabled', true);
                $.ajax({
                    type: "POST",
                    url: "/csv/load",
                    contentType: false,
                    processData: false,
                    data: data
                });
            });
        });
    });

    $.connection.hub.error(function (error) {
        console.log('SignalR error: ' + error)
    });
});