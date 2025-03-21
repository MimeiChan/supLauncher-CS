// menuData オブジェクトはC#側から提供される
// 起動時に自動的にメニューボタンを生成
document.addEventListener('DOMContentLoaded', async function() {
    try {
        // C#側からメニューデータを読み込む
        await loadMenuItems();
    } catch (error) {
        console.error('メニューデータの読み込みに失敗しました:', error);
        document.getElementById('menu-container').innerHTML = `
            <div style="color: red; padding: 20px; text-align: center;">
                <p>メニューデータの読み込みに失敗しました</p>
                <p>${error.message || error}</p>
            </div>
        `;
    }
});

// メニュー項目を読み込み、ボタンを生成する関数
async function loadMenuItems() {
    // C#側からデータを取得
    const menuItems = await window.chrome.webview.hostObjects.menuData.getMenuItems();
    const lockMode = await window.chrome.webview.hostObjects.menuData.isLockMode();
    const cancelButton = await window.chrome.webview.hostObjects.menuData.getCancelButtonIndex();
    
    const container = document.getElementById('menu-container');
    container.innerHTML = ''; // コンテナをクリア
    
    // ボタンを生成
    let visibleCount = 0;
    
    for (let i = 0; i < menuItems.length; i++) {
        const item = menuItems[i];
        
        // 実行モードでは非表示項目はスキップ
        if (lockMode && item.noUse) {
            continue;
        }
        
        const button = document.createElement('button');
        button.className = 'menu-button';
        button.style.setProperty('--index', visibleCount++);
        
        // タイトル設定
        let title = item.title;
        
        // 編集モードでの表示
        if (!lockMode) {
            if (cancelButton === i) {
                button.classList.add('escape');
                title = '＜ＥＳＣ＞' + title;
            }
            
            if (item.noUse) {
                button.classList.add('hidden');
                title = '＜非表示＞' + title;
            }
        }
        
        button.textContent = title;
        
        // ボタンクリックイベント
        button.addEventListener('click', function() {
            // C#側にイベントを通知
            window.chrome.webview.postMessage({
                eventType: 'buttonClick',
                buttonIndex: i
            });
        });
        
        // マウスオーバーでコメントを表示
        if (item.comment) {
            button.addEventListener('mouseenter', function() {
                window.chrome.webview.postMessage({
                    eventType: 'buttonHover',
                    buttonIndex: i
                });
            });
        }
        
        container.appendChild(button);
    }
}

// C#側からUIを更新する命令を受け取るハンドラを登録
window.chrome.webview.addEventListener('message', async function(event) {
    const message = event.data;
    
    switch (message.command) {
        case 'refreshMenu':
            await loadMenuItems();
            break;
        
        case 'updateTheme':
            updateTheme(message.theme);
            break;
    }
});

// テーマカラーを動的に更新する関数
function updateTheme(theme) {
    const root = document.documentElement;
    
    if (theme.primaryColor) root.style.setProperty('--primary-color', theme.primaryColor);
    if (theme.primaryHover) root.style.setProperty('--primary-hover', theme.primaryHover);
    if (theme.secondaryColor) root.style.setProperty('--secondary-color', theme.secondaryColor);
    if (theme.textColor) root.style.setProperty('--text-color', theme.textColor);
    if (theme.backgroundColor) root.style.setProperty('--background-color', theme.backgroundColor);
    if (theme.buttonText) root.style.setProperty('--button-text', theme.buttonText);
}