// console.log($("#mainContents"));
$("#mainContents").change(

    function(){
      $("#mainContents").scrollTop($("#mainContents").prop("scrollHeight"));
      
    }
  )
