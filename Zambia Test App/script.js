// Save original body HTML to restore later
const originalBodyHTML = document.body.innerHTML;

// Onboarding
function completeOnboarding() {
  const username = document.getElementById('username').value;
  const job = document.getElementById('job').value;
  const language = document.getElementById('language').value;

  if (!username || !job || !language) {
    alert("Please complete all fields.");
    return;
  }

  alert("Welcome! You're now registered to Arclight Eye and Ear Care App!");
  showPage('selectModule');
}

// Navigation
function goToDashboard() {
  showPage('dashboard');
}

function showLearningModules() {
  showPage('learningModules');
}

function showAtomsCard() {
  showPage('atomsCardPage');
}

function showPage(pageId) {
  document.querySelectorAll('.page').forEach(page => page.classList.remove('active'));
  document.getElementById(pageId).classList.add('active');
}

// Set up navigation and module clicks after DOM loads
document.addEventListener('DOMContentLoaded', () => {
  document.querySelectorAll('.scroll-item').forEach(item => {
    if (item.textContent.includes('Learning Modules')) {
      item.addEventListener('click', showLearningModules);
    }
  });

  const card = document.getElementById('ophthalmoscopyCard');
  if (card) {
    card.addEventListener('click', () => showPage('directOphthalmoscopy'));
  }

  attachEventListeners();

  // Add event listeners for TOC toggle, back, close buttons
  const tocToggleBtn = document.getElementById('tocToggleBtn');
  if (tocToggleBtn) {
    tocToggleBtn.addEventListener('click', () => {
      const dropdown = document.getElementById('tocDropdown');
      if (dropdown) dropdown.classList.toggle('active');
    });
  }

  const closeTOCBtn = document.getElementById('closeTOCBtn');
  if (closeTOCBtn) {
    closeTOCBtn.addEventListener('click', () => {
      const dropdown = document.getElementById('tocDropdown');
      if (dropdown) dropdown.classList.remove('active');
    });
  }

  const backToEyesBtn = document.getElementById('backToEyesBtn');
  if (backToEyesBtn) {
    backToEyesBtn.addEventListener('click', () => {
      showPage('dashboard');
      const dropdown = document.getElementById('tocDropdown');
      if (dropdown) dropdown.classList.remove('active');
    });
  }

  // Home button listener
  const homeBtn = document.getElementById('homeBtn');
  if (homeBtn) {
    homeBtn.addEventListener('click', () => {
      showPage('dashboard');
    });
  }
});

function showPage(pageId) {
  document.querySelectorAll('.page').forEach(page => page.classList.remove('active'));
  document.getElementById(pageId).classList.add('active');

  const homeBtnContainer = document.getElementById('homeButtonContainer');
  if (!homeBtnContainer) return;

  if (pageId === 'onboarding' || pageId === 'selectModule') {
    homeBtnContainer.style.display = 'none';
  } else {
    homeBtnContainer.style.display = 'flex';
  }
}



// Toolbar setup
function attachEventListeners() {
  const backBtn = document.getElementById('backBtn');
  if (backBtn) backBtn.onclick = () => showPage('learningModules');

  const timestampBtn = document.getElementById('timestampBtn');
  if (timestampBtn) timestampBtn.onclick = showTimestamps;

  const noteBtn = document.getElementById('noteBtn');
  if (noteBtn) noteBtn.onclick = showNote;

  const folderBtn = document.getElementById('folderBtn');
  if (folderBtn) folderBtn.onclick = showFiles;

  const infoBtn = document.getElementById('infoBtn');
  if (infoBtn) infoBtn.onclick = showDefaultInfo;

  const quizBtn = document.getElementById('quizBtn');
  if (quizBtn) quizBtn.onclick = launchQuiz;
}

// YouTube API
let player, infoTimer, lastPauseTime = null;

function onYouTubeIframeAPIReady() {
  player = new YT.Player('ytplayer', {
    events: {
      onStateChange: onPlayerStateChange
    }
  });
}

function onPlayerStateChange(event) {
  clearInterval(infoTimer);
  if (event.data === YT.PlayerState.PLAYING) {
    infoTimer = setInterval(() => {
      const time = Math.floor(player.getCurrentTime());
      if (time === 22 && lastPauseTime !== 'eye-info') {
        lastPauseTime = 'eye-info';
        showEyeAnatomy();
      } else if (time === 23 && lastPauseTime !== 23) {
        lastPauseTime = 23;
        pauseAndShowInfo('pause-only');
      } else if (time === 32 && lastPauseTime !== 32) {
        lastPauseTime = 32;
        pauseAndShowInfo('device');
      }
    }, 1000);
  }
}

function pauseAndShowInfo(type) {
  player.pauseVideo();
  if (type === 'eye') showEyeAnatomy();
  else if (type === 'device') showDeviceImage();

  setTimeout(() => {
    player.playVideo();
  }, 20000);
}

// Toolbar content
const contentBox = document.getElementById('contentBox');

function showTimestamps() {
  setActive('timestampBtn');
  contentBox.innerHTML = `
    <h4>Time stamp</h4>
    <p><a href="#" onclick="seekTo(0)">0:00 General Inspection</a></p>
    <p><a href="#" onclick="seekTo(28)">0:28 Arclight Setup</a></p>
    <p><a href="#" onclick="seekTo(47)">0:47 Red Reflex</a></p>
    <p><a href="#" onclick="seekTo(67)">1:07 Optic Nerve</a></p>
    <p><a href="#" onclick="seekTo(102)">1:42 Retinal Vessels</a></p>
  `;
}

function showNote() {
  setActive('noteBtn');
  contentBox.innerHTML = '<textarea placeholder="Type your notes here..."></textarea>';
}

function showFiles() {
  setActive('folderBtn');
  contentBox.innerHTML = `
    <h4>Attached Files</h4>
    <p><a class="link" href="#">Arclight_Device_Practice.pdf</a></p>
    <p><a class="link" href="#">Red_Reflex.pdf</a></p>
    <p><a class="link" href="#">Ophthalmoscopy_Exercise.docx</a></p>
  `;
}

function showDefaultInfo() {
  setActive('infoBtn');
  contentBox.innerHTML = `
    <h4>Additional Information</h4>
    <p>This video shows how to prepare and use the Arclight ophthalmoscope.</p>
  `;
}

function showEyeAnatomy() {
  contentBox.innerHTML = `
    <h4>Eye Anatomy</h4>
    <img src="https://upload.wikimedia.org/wikipedia/commons/thumb/6/64/Eye_anatomy_diagram.svg/1200px-Eye_anatomy_diagram.svg.png"
    style="width: 100%; border-radius: 5px; margin-top: 10px;" />
    <ul><li>Periorbita</li><li>Eyelids</li><li>Eyes</li></ul>
  `;
}

function showDeviceImage() {
  contentBox.innerHTML = `
    <h4>Arclight Device Overview</h4>
    <img src="images/arclight_device.png" style="width: 100%; border-radius: 5px;" />
  `;
}

function setActive(id) {
  document.querySelectorAll('.toolbar button, .header button').forEach(btn => btn.classList.remove('active'));
  const activeBtn = document.getElementById(id);
  if (activeBtn) activeBtn.classList.add('active');
}

function seekTo(sec) {
  if (player && player.seekTo) {
    player.seekTo(sec, true);
    lastPauseTime = null;
  }
}

// Launch Quiz
function launchQuiz() {
  const quizPageHTML = `
    <div class="quiz-container">
      <div class="quiz-header small">
        <div class="quiz-header-row centered">
          <button id="backToVideoBtn" class="back-icon" title="Go back">←</button>
          <h2>Quiz</h2>
        </div>
      </div>
      <div class="quiz-scroll" id="quizScroll">
        <form id="quizForm"></form>
      </div>
      <div class="quiz-footer">
        <button type="submit" form="quizForm" class="start-btn">See Results</button>
      </div>
      <div id="quizModal" class="quiz-modal hidden">
        <div class="quiz-modal-content">
          <p id="quizScoreText"></p>
          <button id="seeWhyBtn">See why?</button>
        </div>
      </div>
    </div>
  `;

  document.body.innerHTML = quizPageHTML;
  document.body.classList.add('quiz-mode');

  const quizForm = document.getElementById('quizForm');
  const questions = [
    {
      q: "1. When starting direct ophthalmoscopy, what is the ideal distance between the examiner and the patient?",
      options: ["5 cm", "10 cm", "15 cm", "Arm’s length"],
      answer: 3
    },
    {
      q: "2. Which of the options describe the best condition to get the view of the retina?",
      options: ["Outdoors with bright sunlight, dilated pupil", "Deem room with dilated pupil", "Indoors with bright light, dilated pupil", "Deem room with constricted pupil"],
      answer: 1
    },
    {
      q: "3. Which eye should you use to examine the patient’s right eye?",
      options: ["Left eye", "Either eye", "Right eye", "Dominant eye"],
      answer: 2
    },
    {
      q: "4. During ophthalmoscopy, which part of the back of the eye should you identify first?",
      options: ["Macula", "Optic disc", "Retinal periphery", "Fovea"],
      answer: 1
    },
    {
      q: "5. What is the name given to pale optic disc?",
      options: ["Normal finding", "Cataract", "Optic atrophy", "Raised intraocular pressure"],
      answer: 2
    },
    {
      q: "6. Which lighting condition is recommended for performing ophthalmoscopy with the Arclight?",
      options: ["Bright daylight", "Dim or darkened room", "Bright room", "Ambient light"],
      answer: 1
    },
    {
      q: "7. What does a cup-to-disc ratio (CDR) of 0.7 or greater typically suggest on fundus examination?",
      options: ["Glaucoma", "Macular degeneration", "Diabetic retinopathy", "Retinal detachment"],
      answer: 0
    }
  ];

  questions.forEach((q, i) => {
    let block = `<div class="quiz-block"><p>${q.q}</p>`;
    q.options.forEach((opt, j) => {
      block += `
        <label class="radio-label">
          <input type="radio" name="q${i}" value="${j}" />
          <span>${opt}</span>
        </label>`;
    });
    block += `<p class="answer" style="display:none; margin-top:5px; font-style:italic;">Correct answer: ${q.options[q.answer]}</p>`;
    block += `</div>`;
    quizForm.innerHTML += block;
  });

  quizForm.onsubmit = (e) => {
    e.preventDefault();
    let correct = 0;

    questions.forEach((q, i) => {
      const radios = document.querySelectorAll(`input[name="q${i}"]`);
      const answer = q.answer;
      let selected = null;

      radios.forEach(r => {
        r.disabled = true;
        if (r.checked) selected = parseInt(r.value);
      });

      const labels = radios[0].closest('.quiz-block').querySelectorAll('label');
      labels.forEach((label, index) => {
        if (index === answer) {
          label.classList.add('correct');
        } else if (parseInt(label.querySelector('input').value) === selected) {
          label.classList.add('wrong');
        }
      });

      if (selected === answer) correct++;
    });

    document.getElementById('quizScoreText').innerText = `You got ${correct} out of ${questions.length} correct.`;
    document.getElementById('quizModal').classList.remove('hidden');
  };

  document.addEventListener('click', (e) => {
    if (e.target.id === 'seeWhyBtn') {
      document.getElementById('quizModal').classList.add('hidden');
      document.querySelectorAll('.answer').forEach(a => a.style.display = 'block');
    }
    if (e.target.id === 'backToVideoBtn') {
      if (document.getElementById('directOphthalmoscopy')) {
        document.body.innerHTML = originalBodyHTML;
        document.body.classList.remove('quiz-mode');
        attachEventListeners();
        showPage('directOphthalmoscopy');
      } else {
        alert("Oops! 'Direct Ophthalmoscopy' page not found.");
      }
    }
  });
}

// TOC Overlay Logic for Atoms Card Page
function showTOC(type) {
  const tocList = document.getElementById('tocList');
  tocList.innerHTML = '';

  const eyes = [
    'Anatomy', 'Arclight', 'Case Study', 'Child', 'Front of Eye',
    'Fundal Reflex', 'Fundus', 'Glaucoma', 'How to Use',
    'Lens', 'Pupil', 'Red Eye', 'Refract', 'Summary'
  ];

  const ears = ['Drum', 'Ear', 'Ear Anatomy', 'Ear Child'];

  const items = type === 'eyes' ? eyes.sort() : ears.sort();

  items.forEach(item => {
    const li = document.createElement('li');
    li.textContent = item;
    tocList.appendChild(li);
  });

  setupTOCImageSwitch();
  setupSlideSwitch();
}

// Map TOC items to images (single images)
const imageMap = {
  'Anatomy': 'Anatomy1.png',  // default to Anatomy1.png initially
  'Anatomy1.png': 'Anatomy1.png',
  'Anatomy2.png': 'Anatomy2.png',
  'Arclight': 'Arclight.png',
  'Case Study': 'CaseStudy1.png', // default CaseStudy1.png initially
  'CaseStudy1.png': 'CaseStudy1.png',
  'CaseStudy2.png': 'CaseStudy2.png',
  'Child': 'Child.png',
  'Front of Eye': 'FrontOfEye.png',
  'Fundal Reflex': 'FundalReflex.png',
  'Fundus': 'Fundus.png',
  'Glaucoma': 'Glaucoma.png',
  'How to Use': 'HowToUse.png',
  'Lens': 'Lens.png',
  'Pupil': 'Pupil.png',
  'Red Eye': 'RedEye.png',
  'Refract': 'Refract.png',
  'Summary': 'Summary.png',
  'Drum': 'Drum.png',
  'Ear': 'Ear.png',
  'Ear Anatomy': 'EarAnatomy.png',
  'Ear Child': 'EarChild.png',
};

// Attach click handlers to TOC items for image switching
function setupTOCImageSwitch() {
  const tocList = document.getElementById('tocList');
  const atomsImageContainer = document.getElementById('atomsImageContainer');
  if (!atomsImageContainer) return;

  // We'll track currentKey and currentIndex for multi-image items
  let currentKey = '';
  let currentIndex = 0;

  // Multi-image items map
  const multiImageMap = {
    'Anatomy': ['Anatomy1.png', 'Anatomy2.png'],
    'Case Study': ['CaseStudy1.png', 'CaseStudy2.png']
  };

  tocList.querySelectorAll('li').forEach(li => {
    li.style.cursor = 'pointer';
    li.onclick = () => {
      currentKey = li.textContent;
      currentIndex = 0;

      let imgName;

      if (multiImageMap[currentKey]) {
        imgName = multiImageMap[currentKey][currentIndex];
      } else {
        imgName = imageMap[currentKey] || 'Summary.png';
      }

      updateImage(imgName, currentKey);
    };
  });

  // Helper function to update image
  function updateImage(imgName, altText) {
    const existingImg = atomsImageContainer.querySelector('img');
    if (existingImg) {
      existingImg.src = `images/${imgName}`;
      existingImg.alt = altText;
    } else {
      const img = document.createElement('img');
      img.src = `images/${imgName}`;
      img.alt = altText;
      img.style.maxWidth = '100vh';
      img.style.maxHeight = '100vw';
      img.style.transform = 'rotate(90deg)';
      img.style.borderRadius = '12px';
      img.style.objectFit = 'contain';
      atomsImageContainer.innerHTML = '';
      atomsImageContainer.appendChild(img);
    }
  }

  // Expose currentKey and currentIndex for slide switching
  setupSlideSwitch(currentKey, currentIndex, multiImageMap, updateImage);
}

// Slide / scroll switching function
function setupSlideSwitch(currentKeyInit, currentIndexInit, multiImageMap, updateImage) {
  const atomsImageContainer = document.getElementById('atomsImageContainer');
  if (!atomsImageContainer) return;

  // State variables stored in closure
  let currentKey = currentKeyInit || '';
  let currentIndex = currentIndexInit || 0;

  // Listen to wheel event for scroll on desktop
  atomsImageContainer.addEventListener('wheel', (e) => {
    if (!multiImageMap[currentKey]) return;

    e.preventDefault();

    if (e.deltaY > 0) {
      currentIndex = (currentIndex + 1) % multiImageMap[currentKey].length;
    } else {
      currentIndex = (currentIndex - 1 + multiImageMap[currentKey].length) % multiImageMap[currentKey].length;
    }

    updateImage(multiImageMap[currentKey][currentIndex], currentKey);
  }, { passive: false });

  // Touch swipe detection for mobile
  let touchStartY = null;

  atomsImageContainer.addEventListener('touchstart', (e) => {
    touchStartY = e.changedTouches[0].clientY;
  });

  atomsImageContainer.addEventListener('touchend', (e) => {
    if (touchStartY === null) return;

    const touchEndY = e.changedTouches[0].clientY;
    const diffY = touchStartY - touchEndY;

    if (Math.abs(diffY) > 30) { // threshold
      if (!multiImageMap[currentKey]) return;

      if (diffY > 0) {
        currentIndex = (currentIndex + 1) % multiImageMap[currentKey].length;
      } else {
        currentIndex = (currentIndex - 1 + multiImageMap[currentKey].length) % multiImageMap[currentKey].length;
      }

      updateImage(multiImageMap[currentKey][currentIndex], currentKey);
    }

    touchStartY = null;
  });
}

(function(){
  // Scoped selector helper inside quiz container
  const container = document.getElementById('anteriorSegmentQuizModule');
  if (!container) return;

  // Elements inside quiz module
  const caseTitle = container.querySelector("#caseTitle");
  const caseSubtitle = container.querySelector("#caseSubtitle");
  const caseImage = container.querySelector("#caseImage");
  const quizForm = container.querySelector("#quizForm");
  const prevQuestionBtn = container.querySelector("#prevQuestionBtn");
  const nextQuestionBtn = container.querySelector("#nextQuestionBtn");
  const nextCaseBtn = container.querySelector("#nextCaseBtn");
  const scoreCard = container.querySelector("#scoreCard");
  const quizCard = container.querySelector("#quizCard");
  const scoreText = container.querySelector("#scoreText");
  const restartBtn = container.querySelector("#restartBtn");
  const reviewBtn = container.querySelector("#reviewBtn");
  const reviewCard = container.querySelector("#reviewCard");
  const reviewContent = container.querySelector("#reviewContent");
  const closeReviewBtn = container.querySelector("#closeReviewBtn");
  const backBtn = container.querySelector("#anteriorQuizBackBtn");

  // Data - clinical cases and questions (same as original)
  const cases = [
    {
      title: "6 month old baby: 'Eye looks funny'",
      image: "images/case1_eye.png",
      questions: [
        {
          question: "What is the dominant abnormal sign?",
          options: [
            "Hazey/grey cornea",
            "White pupil (leucocoria)",
            "Keratic precipitates",
            "Hypopyon"
          ],
          correctIndex: 1
        },
        {
          question: "What is the most likely diagnosis?",
          options: [
            "Corneal scar",
            "Congenital cataract",
            "Infective keratitis",
            "Limbal dermoid"
          ],
          correctIndex: 1
        },
        {
          question: "What should you do?",
          options: [
            "Prescribe topical antibiotics",
            "Refer urgently to an eye-care professional",
            "Reassure and discharge",
            "Assess for spectacles only"
          ],
          correctIndex: 1
        }
      ]
    },
    // ... include all other cases exactly as in your original quiz JS ...
    // Make sure to copy all cases you provided earlier here!
  ];

  // Copy all the cases here as in your original quiz page

  // State
  let currentCaseIndex = 0;
  let currentQuestionIndex = 0;
  let answers = cases.map(c => new Array(c.questions.length).fill(null));

  // Render question function
  function renderQuestion(caseIndex, questionIndex) {
    const c = cases[caseIndex];
    const q = c.questions[questionIndex];

    caseTitle.textContent = `Case ${caseIndex + 1}`;
    caseSubtitle.textContent = c.title;

    caseImage.src = c.image;
    caseImage.alt = c.title + " image";

    quizForm.innerHTML = "";

    const div = document.createElement("div");
    div.classList.add("question");

    const h3 = document.createElement("h3");
    h3.textContent = `${questionIndex + 1}. ${q.question}`;
    div.appendChild(h3);

    const ul = document.createElement("ul");
    ul.classList.add("options");

    q.options.forEach((opt, optIndex) => {
      const li = document.createElement("li");
      const input = document.createElement("input");
      const id = `case${caseIndex}_q${questionIndex}_opt${optIndex}`;
      input.type = "radio";
      input.name = `q${questionIndex}`;
      input.id = id;
      input.value = optIndex;
      if (answers[caseIndex][questionIndex] === optIndex) {
        input.checked = true;
      }
      input.onchange = () => {
        answers[caseIndex][questionIndex] = parseInt(input.value);
        updateButtons();
      };

      const label = document.createElement("label");
      label.htmlFor = id;
      label.textContent = opt;

      li.appendChild(input);
      li.appendChild(label);
      ul.appendChild(li);
    });

    div.appendChild(ul);
    quizForm.appendChild(div);

    updateButtons();
  }

  // Update nav buttons state and next case button
  function updateButtons() {
    prevQuestionBtn.disabled = currentQuestionIndex === 0;
    nextQuestionBtn.disabled = currentQuestionIndex === cases[currentCaseIndex].questions.length - 1;

    // Show Next Case button if all questions answered for current case
    const allAnswered = !answers[currentCaseIndex].some(a => a === null);
    nextCaseBtn.style.display = allAnswered ? "block" : "none";

    nextCaseBtn.disabled = !allAnswered;
  }

  // Navigation button handlers
  prevQuestionBtn.onclick = () => {
    if (currentQuestionIndex > 0) {
      currentQuestionIndex--;
      renderQuestion(currentCaseIndex, currentQuestionIndex);
    }
  };

  nextQuestionBtn.onclick = () => {
    if (currentQuestionIndex < cases[currentCaseIndex].questions.length - 1) {
      currentQuestionIndex++;
      renderQuestion(currentCaseIndex, currentQuestionIndex);
    }
  };

  nextCaseBtn.onclick = () => {
    currentCaseIndex++;
    currentQuestionIndex = 0;
    if (currentCaseIndex >= cases.length) {
      showScore();
    } else {
      renderQuestion(currentCaseIndex, currentQuestionIndex);
    }
  };

  // Show score page
  function showScore() {
    quizCard.style.display = "none";
    scoreCard.style.display = "block";

    let totalQuestions = 0;
    let correctCount = 0;
    cases.forEach((c, caseIdx) => {
      c.questions.forEach((q, qIdx) => {
        totalQuestions++;
        if (answers[caseIdx] && answers[caseIdx][qIdx] === q.correctIndex) {
          correctCount++;
        }
      });
    });

    scoreText.textContent = `You scored ${correctCount} out of ${totalQuestions} correct.`;
  }

  // Restart quiz
  restartBtn.onclick = () => {
    currentCaseIndex = 0;
    currentQuestionIndex = 0;
    answers = cases.map(c => new Array(c.questions.length).fill(null));
    scoreCard.style.display = "none";
    reviewCard.style.display = "none";
    quizCard.style.display = "block";
    renderQuestion(currentCaseIndex, currentQuestionIndex);
  };

  // Review quiz
  reviewBtn.onclick = () => {
    scoreCard.style.display = "none";
    reviewCard.style.display = "block";
    buildReview();
  };

  closeReviewBtn.onclick = () => {
    reviewCard.style.display = "none";
    scoreCard.style.display = "block";
  };

  // Build review page content
  function buildReview() {
    reviewContent.innerHTML = "";
    cases.forEach((c, caseIdx) => {
      const caseDiv = document.createElement("div");
      caseDiv.classList.add("review-case");

      const h2 = document.createElement("h2");
      h2.textContent = `Case ${caseIdx + 1}`;
      caseDiv.appendChild(h2);

      const subtitle = document.createElement("div");
      subtitle.id = "caseSubtitle";
      subtitle.textContent = c.title;
      caseDiv.appendChild(subtitle);

      const img = document.createElement("img");
      img.className = "case-image";
      img.src = c.image;
      img.alt = c.title + " image";
      caseDiv.appendChild(img);

      c.questions.forEach((q, qIdx) => {
        const qDiv = document.createElement("div");
        qDiv.classList.add("review-question");

        const qH3 = document.createElement("h3");
        qH3.textContent = `${qIdx + 1}. ${q.question}`;
        qDiv.appendChild(qH3);

        const ul = document.createElement("ul");
        ul.classList.add("review-options");

        q.options.forEach((opt, optIdx) => {
          const li = document.createElement("li");

          if (optIdx === q.correctIndex) {
            li.classList.add("correct");
          }
          if (answers[caseIdx][qIdx] === optIdx) {
            li.classList.add("user-selected");
          }

          li.textContent = opt;
          ul.appendChild(li);
        });

        qDiv.appendChild(ul);
        caseDiv.appendChild(qDiv);
      });

      reviewContent.appendChild(caseDiv);
    });
  }

  // Initialize first question display
  function startAnteriorSegmentQuiz() {
    currentCaseIndex = 0;
    currentQuestionIndex = 0;
    answers = cases.map(c => new Array(c.questions.length).fill(null));
    scoreCard.style.display = "none";
    reviewCard.style.display = "none";
    quizCard.style.display = "block";
    renderQuestion(currentCaseIndex, currentQuestionIndex);
  }

  // Back button to return to learning modules
  backBtn.onclick = () => {
    showPage('learningModules');
  };

  // Expose start function globally so it can be called from outside
  window.startAnteriorSegmentQuiz = startAnteriorSegmentQuiz;
})();

// Navigation utility
function showPage(pageId) {
  document.querySelectorAll('.page').forEach(page => page.classList.remove('active'));
  const page = document.getElementById(pageId);
  if (page) page.classList.add('active');

  const homeBtnContainer = document.getElementById('homeButtonContainer');
  if (!homeBtnContainer) return;

  if (pageId === 'onboarding' || pageId === 'selectModule') {
    homeBtnContainer.style.display = 'none';
  } else {
    homeBtnContainer.style.display = 'flex';
  }
}

// Initial navigation setup after DOM loads
document.addEventListener('DOMContentLoaded', () => {
  // Click on Anterior Segment card opens quiz iframe page
  const anteriorCard = document.getElementById('anteriorSegmentCard');
  if (anteriorCard) {
    anteriorCard.style.cursor = 'pointer';
    anteriorCard.addEventListener('click', () => {
      showPage('anteriorSegmentQuizPage');
    });
  }

  // Back button on iframe quiz page
  const quizBackBtn = document.getElementById('quizBackBtn');
  if (quizBackBtn) {
    quizBackBtn.addEventListener('click', () => {
      showPage('learningModules');
    });
  }

  // Home button returns to dashboard
  const homeBtn = document.getElementById('homeBtn');
  if (homeBtn) {
    homeBtn.addEventListener('click', () => {
      showPage('dashboard');
    });
  }

  // ...Other existing event listeners for your app here...
});

// YouTube iframe API, toolbar functions, and any other app JS go here...

// You can keep all your existing functions as is

