
var CacheTableCreator = {
    drawDirectMappedCache: function() {
        $("#cacheTableContainer").empty();

        var cacheTable = `<table border=1 width=100%>
                                        <tr align=center>
                                            <td width=15%>Index</td>
                                            <td width=15%>Valid</td>
                                            <td>Tag</td>
                                            <td>Data</td>
                                        </tr>`;

        var cacheSize = $("#cacheSize").val();
        var dataBlockSize = $("#dataSize").val();

        var indexCount = cacheSize / dataBlockSize;

        for (var index = 0; index < indexCount; index++) {
            cacheTable += "<tr id=\"cacheRow-" + index + "\" align=center>" +
                "<td>" + index + "</td>" +
                "<td id=\"valid-" + index + "\">0</td>" +
                "<td id=\"tag-" + index + "\"></td>" +
                "<td id=\"data-" + index + "\"></td></tr>";
        }

        cacheTable += "</table>";

        $("#cacheTableContainer").html(cacheTable);

        MemoryTableCreator.drawMemory();
    }
}


var MemoryTableCreator = {
    numberOfColumns: 8,

    drawMemory: function () {
        $("#memoryTableContainer").empty();
        var memorySize = $("#memorySize").val();

        var memoryTable = "<table border=1>";

        var entriesPerColumn = memorySize / MemoryTableCreator.numberOfColumns;

        var blockCount = 0;
        for (var row = 0; row < entriesPerColumn; row++) {
            memoryTable += "<tr align=center>";
            for (var column = 0; column < MemoryTableCreator.numberOfColumns; column++) {
                memoryTable += "<td id=\"memory-" + blockCount + "\">B " + blockCount + "</td>";
                blockCount++;
            }
            memoryTable += "</tr>";
        }

        memoryTable += "</table>";

        $("#memoryTableContainer").html(memoryTable);
    }
}
