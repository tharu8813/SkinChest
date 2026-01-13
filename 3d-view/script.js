function handleImageSelect(imagePath) {
    const imageUrl = imagePath;
    setProperty("--skin", 'url("' + imageUrl + '")');
}


function setProperty(property, value) {
  document.documentElement.style.setProperty(property, value);
}

/* ===============================
   전체 크기 조절용 변수
   =============================== */
const BASE_SCALE = 2.5; // ← 이것만 바꾸면 전체 크기 변경됨

let rotation = 0;
const scene = document.querySelector(".scene");

/* 화면 크기에 따른 자동 스케일 */
function resizeScene() {
    const vw = window.innerWidth;
    const vh = window.innerHeight;

    const autoScale = Math.min(vw, vh) / 120;
    const scale = autoScale * BASE_SCALE;

    scene.style.transform =
        `scale(${scale}) rotateY(${rotation}deg)`;
}

/* 자동 회전 */
function animate() {
    rotation += 0.2; // 회전 속도
    resizeScene();
    requestAnimationFrame(animate);
}

window.addEventListener("resize", resizeScene);

resizeScene();
animate();

