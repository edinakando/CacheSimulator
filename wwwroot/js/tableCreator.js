
var TableCreator = {
    numberOfColumns: 8,

    drawCacheAndMemory: function (indexCount, setCount) {
        var cacheTable = "";
        for (var set = 0; set < setCount; set++) {
            cacheTable += this.drawCache(indexCount, set)
        }

        $("#cacheTableContainer").empty();
        $("#cacheTableContainer").html(cacheTable);

        this.drawMemory();
    },

    drawCache: function (indexCount, setCount) {
        var cacheTable = `<table border=1 width=100%>
                                        <tr align=center>
                                            <td width=15%>Index</td>
                                            <td width=15%>Valid</td>
                                            <td>Tag</td>
                                            <td>Data</td>
                                        </tr>`;

        for (var index = 0; index < indexCount; index++) {
            cacheTable += "<tr id=\"set-" + setCount + "-cacheRow-" + index + "\" align=center>" +
                "<td>" + index + "</td>" +
                "<td id=\"set-" + setCount + "-valid-" + index + "\">0</td>" +
                "<td id=\"set-" + setCount + "-tag-" + index + "\"></td>" +
                "<td id=\"set-" + setCount + "-data-" + index + "\"></td></tr>";
        }

        cacheTable += "</table><br/>";

        return cacheTable;
    },

    drawMemory: function () {
        $("#memoryTableContainer").empty();
        var memorySize = $("#memorySize").val();

        var memoryTable = "<table border=1>";

        var entriesPerColumn = memorySize / this.numberOfColumns;

        var blockCount = 0;
        memoryTable += `<tr class=\"gray-highlight\" align=center><td></td><td>0</td><td>1</td><td>2</td><td>3</td>
                                        <td>4</td><td>5</td><td>6</td><td>7</td>`;
        for (var row = 0; row < entriesPerColumn; row++) {
            memoryTable += "<tr align=center>";
            memoryTable += "<td class=\"gray-highlight\" >A " + row + "</td>";
            for (var column = 0; column < this.numberOfColumns; column++) {
                memoryTable += "<td id=\"memory-" + blockCount + "\">D " + blockCount + "</td>";
                blockCount++;
            }
            memoryTable += "</tr>";
        }

        memoryTable += "</table>";

        $("#memoryTableContainer").html(memoryTable);
    }
}




