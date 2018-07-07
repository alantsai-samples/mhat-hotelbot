# 參與 mhat-hotelbot

首先，感謝您對於參與貢獻有興趣。這份文件將會說明如何能夠參與到貢獻。

如果您對於專案結構或者連應該有的內容有任何建議，或者發現一些錯誤，都非常歡迎您提出來。

## 發現bug或者想提建議

如果您在這個專案裡面找到任何bug或者想要針對任意地方提出建議/意見，歡迎透過[開issue](https://github.com/alantsai/mhat-hotelbot/issues/new) 

## 如何開始修改

如果您對於任何地方有建議並且希望您的修改能夠整合回來，依照修改大的不同可以分為：

1. 透過Github直接修改
2. 透過Fork和Pull Reuqest的方式

### 透過Github直接修改

任何檔案只要您想要修改，可以直接點選 **Edit** 的按鈕，並且直接在瀏覽器上面編輯修改。

修改完之後會自動建立一個Pull Request接下來則會有相關管理人員做最後的檢查並且做出是否合併的決定。

### 透過Fork和Pull Request的方式

如果修改的內容大到無法直接在Github界面上面修改，那麼修改流程如下：
1. 先點右上角的 `Fork` 建立一個您的私人專案
2. 從您的私人專案上面做`Clone`把內容抓下來
3. 在您本地做完調整
4. `push`到您的專案上面
5. 建立一個`Pull request`
6. 接下來有管理人員做最後檢查並且決定是否合併。

如果您對pull request不熟悉，可以參考以下資訊（英文）

**Working on your first Pull Request?** You can learn how from this *free* series [How to Contribute to an Open Source Project on GitHub](https://egghead.io/series/how-to-contribute-to-an-open-source-project-on-github)

### Roadmap

對於這個專案目前構想大約有2個階段

1. 把需要的檔案和內容列出來，並且提供以這個專案為例子作為一個範例
2. 用建立template的方式讓產生這個結果變得簡單，目前想法是透過 `yeoman` 和 `dotnet new` 的兩個templateengine來做