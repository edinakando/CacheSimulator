var FullyAssociativeSimulation = {
    currentStep: 0,
    indexCount: 0,
    validBlocks: [],

    startSimulation: function (simulationParameters) {
        CacheTableCreator.drawDirectMappedCache();  

        $.ajax({
            type: "POST",
            async: false,
            url: "/FullyAssociativeCacheSimulation/FullyAssociativeCache",
            data: { simulationParameters: simulationParameters },
            success: function (response) {
                FullyAssociativeSimulation.indexCount = response;
                FullyAssociativeSimulation.nextStep();
            },
            error: function (response) {
                console.log(response);
            }
        });
    },

    nextStep: function () {
        this.currentStep++;

        if (this.currentStep == 1) {
            $.notify("Adress breakdown", "success");
            this.addressBreakdown();
        }
        else if (this.currentStep == 2) {
            $.notify("Finding all blocks where the valid bit is 1", "success");

            for (var index = 0; index < this.indexCount; index++) {
                if ($("#valid-" + index).text() == 1) {
                    $("#cacheRow-" + index).addClass('highlight');
                    this.validBlocks.push(index);
                }
            }
        }
        else if (this.currentStep == 3) {
            if (this.validBlocks.length == 0) {
                $.notify("No valid blocks found => Cache MISS", "error");
                for (var index = 0; index < FullyAssociativeSimulation.indexCount; index++) {
                    $("#valid-" + index).addClass('highlight-red');
                }
            }
        }
        //else if (this.currentStep == 4) {
        //    var valid = $("#valid-" + this.index).text();
        //    if (valid == 0) {
        //        $.notify("Valid bit is 0 => Cache Miss", "error");
        //        $("#valid-" + this.index).addClass('highlight-red');
        //    }
        //    else {
        //        $.notify("Valid bit is 1 => Compare the Tags", "success");

        //        $("#valid-" + this.index).removeClass('highlight-more');
        //        $("#tagValue").addClass('highlight-more');
        //        $("#tag-" + this.index).addClass('highlight-more');

        //        this.isValid = 1;
        //    }
        //}
        //else if (this.currentStep == 5 && this.isValid == 0) {
        //    $.notify("Updating cache", "success");
        //    $("#valid-" + this.index).removeClass('highlight-red');
        //    $("#valid-" + this.index).removeClass('highlight-more');

        //    DirectMappedSimulation.updateCache();
        //}
        //else if (this.currentStep == 5 && this.isValid == 1) {  //Compare tag
        //    var currentMemoryTag = $("#tagValue").text();
        //    var currentCacheTag = $("#tag-" + this.index).text();

        //    if (currentMemoryTag == currentCacheTag) {
        //        $.notify("Tags are equal => Cache HIT", "success");
        //        this.isHit = 1;
        //    }
        //    else {
        //        $.notify("Tags are different => Cache MISS", "error");
        //        $("#tagValue").addClass('highlight-red');
        //        $("#tag-" + this.index).addClass('highlight-red');
        //    }

        //    this.isValid = 0;
        //}
        //else if (this.currentStep == 6 && this.isHit == 0) {
        //    $.notify("Updating cache", "success");

        //    DirectMappedSimulation.updateCache();
        //}
        //else {
        //    this.isHit = 0;
        //    DirectMappedSimulation.updateButtons();
        //}
    },

    addressBreakdown: function () {
        $.ajax({
            type: 'GET',
            async: false,
            url: '/FullyAssociativeCacheSimulation/GetCurrentAddressBreakdown',
            success: function (response) {
                $("#tagValue").text(response.tagBinary);
                $("#addressRow").addClass('highlight');

                //DirectMappedSimulation.index = parseInt(response.indexBinary, 2); //go through all
            }
        });
    },

}