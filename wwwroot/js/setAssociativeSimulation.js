var SetAssociativeSimulation = {
    currentStep: 0,
    currentInstruction: 0,
    indexCount: 0,
    index: 0,
    setCount: 0,
    validBlocks: [],
    existValidBlocks: 0,
    lastUpdatedSet: 0,
    isComparingTags: 0,
    isHit: 0,

    startSimulation: function (simulationParameters) {
        $.ajax({
            type: "POST",
            async: false,
            url: "/SetAssociativeCacheSimulation/SetAssociativeCache",
            data: { simulationParameters: simulationParameters },
            success: function (response) {
                SetAssociativeSimulation.indexCount = response;
            },
            error: function (response) {
                console.log(response);
            }
        });

        TableCreator.drawCacheAndMemory(SetAssociativeSimulation.indexCount, simulationParameters.SetCount);
        this.setCount = simulationParameters.SetCount;
        SetAssociativeSimulation.nextStep();
    },

    nextStep: function () {
        this.currentStep++;

        if (this.currentStep == 1) {
            $.notify("Adress breakdown", "success");
            this.addressBreakdown();
        }
        else if (this.currentStep == 2) {
            $.notify("Checking corresponding index", "success")
            for (var set = 0; set < this.setCount; set++) {
                $("#set-" + set + "-cacheRow-" + this.index).addClass('highlight');
            }
        }
        else if (this.currentStep == 3) {
            $.notify("Finding all blocks where the valid bit is 1", "success");

            for (var set = 0; set < this.setCount; set++) {
                if ($("#set-" + set + "-valid-" + this.index).text() == 1) {
                    $("#set-" + set + "-valid-" + this.index).addClass('highlight-more');
                    this.validBlocks.push(set);
                    this.existValidBlocks = 1;
                }
            }
        }
        else if (this.currentStep == 4) {
            if (this.existValidBlocks) {
                $.notify("Comparing corresponding tags", "success");
                $("#tagValue").addClass('highlight-more');
                for (var set = 0; set < this.validBlocks.length; set++) {
                    $("#set-" + this.validBlocks[set] + "-valid-" + this.index).removeClass('highlight-more');
                    $("#set-" + this.validBlocks[set] + "-tag-" + this.index).addClass('highlight-more');
                    this.isComparingTags = 1;
                }
            }
            else{
                $.notify("No valid blocks found => Cache MISS", "error");
                for (var set = 0; set < this.setCount; set++) {
                    $("#set-" + set + "-valid-" + this.index).addClass('highlight-red');
                }
            }
        }
        else if (this.currentStep == 5) {
            if (this.isComparingTags) {
                for (var set = 0; set < this.validBlocks.length; set++) {
                    if ($("#set-" + this.validBlocks[set] + "-tag-" + this.index).text() == $("#tagValue").text()) {
                        $.notify("Equal tags found => Cache HIT", "success");
                        $("#set-" + this.validBlocks[set] + "-tag-" + this.index).removeClass('highlight-more')
                        $("#set-" + this.validBlocks[set] + "-cacheRow-" + this.index).addClass('highlight-hit');
                        this.isHit = 1;

                        //$.ajax({
                        //    type: 'POST',
                        //    async: false,
                        //    url: '/FullyAssociativeCacheSimulation/CacheHit',
                        //    data: { index: FullyAssociativeSimulation.validBlocks[i] }
                        //});
                        break;
                    }
                }

                if (this.isHit == 0) {
                    $.notify("Tags don't match => Cache MISS", "error");
                    for (var set = 0; set < this.validBlocks.length; set++) {
                        $("#set-" + this.validBlocks[set] + "-tag-" + this.index).addClass('highlight-red')
                        $("#tagValue").addClass('highlight-red')
                    }
                }
            }
            else {
                SetAssociativeSimulation.removeAllHighlightFromCache();
                SetAssociativeSimulation.updateCache();
            }
        }
        else if (this.currentStep == 6) {
            if (this.isHit) {
                Simulation.updateButtons();
            }
            else {
                SetAssociativeSimulation.removeAllHighlightFromCache();
                SetAssociativeSimulation.updateCache();
            }
        }
    },

    addressBreakdown: function () {
        $.ajax({
            type: 'GET',
            async: false,
            url: '/SetAssociativeCacheSimulation/GetCurrentAddressBreakdown',
            success: function (response) {
                $("#addressRow").addClass('highlight');
                $("#tagValue").text(response.tagBinary);
                $("#indexValue").text(response.indexBinary);
                $("#offsetValue").text(response.offsetBinary);

                SetAssociativeSimulation.index = parseInt(response.indexBinary, 2);
            }
        });
    },

    removeAllHighlightFromCache: function () {
        for (var set = 0; set < this.setCount; set++) {
            for (var cacheLine = 0; cacheLine < this.indexCount; cacheLine++) {
                $("#set-" + set + "-cacheRow-" + cacheLine).removeAttr('class');
                $("#set-" + set + "-tag-" + cacheLine).removeAttr('class');
                $("#set-" + set + "-valid-" + cacheLine).removeAttr('class');
            }
        }
    },

    updateCache() {
        $.ajax({
            type: 'GET',
            url: 'SetAssociativeCacheSimulation/UpdateCache',
            async: false,
            success: function (response) {
                $.notify(response.cache[response.lastUpdatedSet].cacheUpdateTypeMessage, "success");
                var lastUpdatedCacheLine = SetAssociativeSimulation.index;
                var cache = response.cache[response.lastUpdatedSet];
                $("#set-" + response.lastUpdatedSet + "-valid-" + lastUpdatedCacheLine).text('1');
                $("#set-" + response.lastUpdatedSet + "-tag-" + lastUpdatedCacheLine).text(cache.tags[lastUpdatedCacheLine]);
                $("#set-" + response.lastUpdatedSet + "-data-" + lastUpdatedCacheLine).text(cache.cacheLines[lastUpdatedCacheLine].data.toString());

                SetAssociativeSimulation.lastUpdatedSet = response.lastUpdatedSet;
                Simulation.currentMemoryAddress = cache.currentMemoryAddress * Simulation.cacheLineSize;

                $("#set-" + response.lastUpdatedSet + "-cacheRow-" + SetAssociativeSimulation.index).addClass('highlight');

                for (var i = Simulation.currentMemoryAddress; i < Simulation.currentMemoryAddress + Simulation.cacheLineSize; i++) {
                    $("#memory-" + i).addClass('highlight');
                }

                Simulation.updateButtons();
            }
        });
    },

    nextInstruction: function () {
        this.currentStep = 0;
        this.currentInstruction++;

        $("#nextStepButton").attr("hidden", false);
        $("#nextInstructionButton").attr("hidden", true);

        $("#addressRow").removeAttr('class');
        $("#tagValue").removeAttr('class');
        SetAssociativeSimulation.removeAllHighlightFromCache();

        for (var i = Simulation.currentMemoryAddress; i < Simulation.currentMemoryAddress + Simulation.cacheLineSize; i++) {
            $("#memory-" + i).removeClass('highlight');
        }

        Simulation.highlightCurrentInstruction(this.currentInstruction);

        this.existValidBlocks = 0;
        this.isComparingTags = 0;
        //this.isHit = 0;

        $.ajax({
            type: 'POST',
            url: 'SetAssociativeCacheSimulation/NextInstruction'
        });
    },

    finish: function () {
        location.reload();

        $.ajax({
            type: 'POST',
            url: "/SetAssociativeCacheSimulation/Reset"
        });
    },
    
}