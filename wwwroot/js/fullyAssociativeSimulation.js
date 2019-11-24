var FullyAssociativeSimulation = {
    currentStep: 0,
    currentInstruction: 0,
    indexCount: 0,
    validBlocks: [],
    lastUpdatedIndex: 0,
    isComparingTags: 0,
    existValidBlocks: 0,
    isHit: 0,

    startSimulation: function (simulationParameters) {
        $.ajax({
            type: "POST",
            async: false,
            url: "/FullyAssociativeCacheSimulation/FullyAssociativeCache",
            data: { simulationParameters: simulationParameters },
            success: function (response) {
                FullyAssociativeSimulation.indexCount = response;
            },
            error: function (response) {
                console.log(response);
            }
        });

        TableCreator.drawCacheAndMemory(FullyAssociativeSimulation.indexCount, 1);
        FullyAssociativeSimulation.nextStep();
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
                if ($("#set-0-valid-" + index).text() == 1) {
                    $("#set-0-cacheRow-" + index).addClass('highlight');
                    this.validBlocks.push(index);
                    this.existValidBlocks = 1;
                }
            }
        }
        else if (this.currentStep == 3) {
            if (this.existValidBlocks) {
                $.notify("Valid blocks found", "success");
                for (var index = 0; index < this.validBlocks.length; index++) {
                    $("#set-0-valid-" + this.validBlocks[index]).addClass('highlight-more');
                }
            }
            else {
                $.notify("No valid blocks found => Cache MISS", "error");
                for (var index = 0; index < FullyAssociativeSimulation.indexCount; index++) {
                    $("#set-0-valid-" + index).addClass('highlight-red');
                }
            }
        }
        else if (this.currentStep == 4) {
            if (this.existValidBlocks) {
                $.notify("Comparing corresponding tags", "success");
                $("#tagValue").addClass('highlight-more');
                for (var index = 0; index < this.validBlocks.length; index++) {
                    $("#set-0-valid-" + this.validBlocks[index]).removeClass('highlight-more');
                    $("#set-0-tag-" + this.validBlocks[index]).addClass('highlight-more');
                    this.isComparingTags = 1;
                }
            }
            else {
                FullyAssociativeSimulation.removeAllHighlightFromCache();
                FullyAssociativeSimulation.updateCache();
            }
        }
        else if (this.currentStep == 5) {
            if (this.isComparingTags) {
                for (var i = 0; i < this.validBlocks.length; i++) {
                    if ($("#set-0-tag-" + this.validBlocks[i]).text() == $("#tagValue").text()) {
                        $.notify("Equal tags found => Cache HIT", "success");
                        $("#set-0-tag-" + this.validBlocks[i]).removeClass('highlight-more')
                        $("#set-0-cacheRow-" + this.validBlocks[i]).addClass('highlight-hit');
                        this.isHit = 1;

                        $.ajax({
                            type: 'POST',
                            async: false,
                            url: '/FullyAssociativeCacheSimulation/CacheHit',
                            data: { index: FullyAssociativeSimulation.validBlocks[i] }
                        });

                        break;
                    }
                }

                if (this.isHit == 0) {
                    $.notify("Tags don't match => Cache MISS", "error");
                    for (var i = 0; i < this.validBlocks.length; i++) {
                        $("#set-0-tag-" + this.validBlocks[i]).addClass('highlight-red')
                        $("#tagValue").addClass('highlight-red')
                    }
                }
            }
        }
        else if (this.currentStep == 6) {
            if (this.isHit) {
                Simulation.updateButtons();
            }
            else {
                FullyAssociativeSimulation.removeAllHighlightFromCache();
                FullyAssociativeSimulation.updateCache();
            }
        }
    },

    addressBreakdown: function () {
        $.ajax({
            type: 'GET',
            async: false,
            url: '/FullyAssociativeCacheSimulation/GetCurrentAddressBreakdown',
            success: function (response) {
                $("#addressRow").addClass('highlight');
                $("#tagValue").text(response.tagBinary);
                $("#offsetValue").text(response.offsetBinary);
            }
        });
    },

    updateCache() {
        $.ajax({
            type: 'GET',
            url: 'FullyAssociativeCacheSimulation/UpdateCache',
            async: false,
            success: function (response) {
                $.notify(response.cacheUpdateTypeMessage, "success");
                $("#set-0-valid-" + response.lastUpdatedIndex).text('1');
                $("#set-0-tag-" + response.lastUpdatedIndex).text(response.tags[response.lastUpdatedIndex]);
                $("#set-0-data-" + response.lastUpdatedIndex).text(response.cacheLines[response.lastUpdatedIndex].data.toString());

                FullyAssociativeSimulation.lastUpdatedIndex = response.lastUpdatedIndex;
                Simulation.currentMemoryAddress = response.currentMemoryAddress * Simulation.cacheLineSize;

                $("#set-0-cacheRow-" + FullyAssociativeSimulation.lastUpdatedIndex).addClass('highlight');

                for (var i = Simulation.currentMemoryAddress; i < Simulation.currentMemoryAddress + Simulation.cacheLineSize; i++) {
                    $("#memory-" + i).addClass('highlight');
                }

                for (var index = 0; index < FullyAssociativeSimulation.indexCount; index++) {
                    $("#set-0-valid-" + index).removeClass('highlight-red');
                }

                Simulation.updateButtons();
            }
        });
    },

    finish: function () {
        location.reload();

        $.ajax({
            type: 'POST',
            url: "/FullyAssociativeCacheSimulation/Reset"
        });
    },

    nextInstruction: function () {
        this.currentStep = 0;
        this.currentInstruction++;

        $("#nextStepButton").attr("hidden", false);
        $("#nextInstructionButton").attr("hidden", true);

        $("#addressRow").removeAttr('class');
        $("#tagValue").removeAttr('class');
        FullyAssociativeSimulation.removeAllHighlightFromCache();

        for (var i = Simulation.currentMemoryAddress; i < Simulation.currentMemoryAddress + Simulation.cacheLineSize; i++) {
            $("#memory-" + i).removeClass('highlight');
        }

        Simulation.highlightCurrentInstruction(this.currentInstruction);

        this.existValidBlocks = 0;
        this.isComparingTags = 0;
        this.isHit = 0;

        $.ajax({
            type: 'POST',
            url: 'FullyAssociativeCacheSimulation/NextInstruction'
        });
    },

    removeAllHighlightFromCache: function () {
        for (var cacheLine = 0; cacheLine < this.indexCount; cacheLine++) {
            $("#set-0-cacheRow-" + cacheLine).removeAttr('class');
            $("#set-0-tag-" + cacheLine).removeAttr('class');
            $("#set-0-valid-" + cacheLine).removeAttr('class');
        }
    },
}