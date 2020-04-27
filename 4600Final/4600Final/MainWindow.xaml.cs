//got how to write to a file from https://docs.microsoft.com/en-us/dotnet/standard/io/how-to-write-text-to-a-file
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HtmlAgilityPack;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO;

namespace _4600Final
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static string url;
        private static string county;

        public MainWindow()
        {
            InitializeComponent();
        }

        private static async void GetHtml()
        {
            //create the http client and load the url
            var httpclient = new HttpClient();
            var html = await httpclient.GetStringAsync(url);

            //create a html document to store the html code
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(html);

            //parse the new html code
            ParseHtml(htmlDocument);
        }

        private static void ParseHtml(HtmlDocument htmlDocument)
        {
            //list for all houses
            var houseList = htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Contains("listings-grid")).ToList();

            //list of house elements 
            var houses = houseList[0].Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Contains("listings-card")).ToList();

            //idividual houses
            var house = htmlDocument.DocumentNode.Descendants("div")
                .Where(node => node.GetAttributeValue("class", "")
                .Contains("card-details-stats")).ToList();

            //list of house specs
            var numOfBeds = htmlDocument.DocumentNode.Descendants("p")
                 .Where(node => node.GetAttributeValue("class", "")
                 .Equals("card-details-stat")).ToList();

            GetHouseData(htmlDocument, houses, numOfBeds);
        }

        private static void GetHouseData(HtmlDocument htmlDocument, List<HtmlNode> houses, List<HtmlNode> numOfBeds)
        {

            List<House> listOfHomes = new List<House>();
            int j = 1;

            foreach (var homes in houses)
            {
                House home = new House();

                //address
                home.address = (homes.Descendants("h5")
                     .Where(node => node.GetAttributeValue("class", "")
                     .Equals("card-address-street")).FirstOrDefault().InnerText.Trim());

                //County
                home.County = (homes.Descendants("h6")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("card-address-location")).FirstOrDefault().InnerText.Trim());

                //price
                home.price = (homes.Descendants("h4")
                    .Where(node => node.GetAttributeValue("class", "")
                    .Equals("price")).FirstOrDefault().InnerText.Trim());

                getList(htmlDocument, numOfBeds, j, home);
                j++;

                listOfHomes.Add(home);

            }

            WriteDataToFile(listOfHomes);

        }

        private static void getList(HtmlDocument htmlDocument, List<HtmlNode> numOfBeds, int j, House home)
        {
            home.numberOfBeds = (htmlDocument.DocumentNode.SelectNodes("//*[@id='__layout']/div/div[2]/main/div/section[2]/div[2]/div/div[1]/div[" + j + "]/div/div[2]/div[2]/p[1]").FirstOrDefault().InnerText.Trim());
            home.numberOfBaths = (htmlDocument.DocumentNode.SelectNodes("//*[@id='__layout']/div/div[2]/main/div/section[2]/div[2]/div/div[1]/div[" + j + "]/div/div[2]/div[2]/p[2]").FirstOrDefault().InnerText.Trim());
            home.sqft = (htmlDocument.DocumentNode.SelectNodes("//*[@id='__layout']/div/div[2]/main/div/section[2]/div[2]/div/div[1]/div[" + j + "]/div/div[2]/div[2]/p[3]").FirstOrDefault().InnerText.Trim());
        }

        private static void WriteDataToFile(List<House> listOfHomes)
        {
            var docPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            using (StreamWriter outputFile = new StreamWriter(System.IO.Path.Combine(docPath, county + ".txt")))

            {
               // outputFile.WriteLine(ProductHtmles);
                foreach (House home in listOfHomes)
                {
                    outputFile.WriteLine(home.address);
                    outputFile.WriteLine(home.price);
                    outputFile.WriteLine(home.County);
                    outputFile.WriteLine(home.numberOfBeds);
                    outputFile.WriteLine(home.numberOfBaths);
                    outputFile.WriteLine(home.sqft);
                }
            }
        }

        public void ListBoxItem_Clarksville(object sender, RoutedEventArgs e)
        {
            url = "https://www.remax.com/homes-for-sale/TN/Clarksville/city/4715160?filters={%22place%22:{%22lat%22:36.560306,%22lon%22:-87.346454,%22city%22:%22Clarksville%22,%22state%22:%22TN%22,%22placename%22:%22Clarksville,%20TN%22,%22placeType%22:%22city%22,%22placeId%22:%224715160%22,%22areaSquareMiles%22:98.05},%22locationRect%22:{%22minLat%22:36.641767,%22maxLat%22:36.478845,%22minLon%22:-87.482243,%22maxLon%22:-87.210666},%22bPropertyType%22:[%22Single%20Family%22,%22Condo/Townhome%22,%22Mobile%20Home%22,%22Multi-Family%22,%22Rental%22,%22Farm%22,%22Land%22],%22bStatus%22:[%22For%20Sale%22,%22Under%20Contract%22],%22zoom%22:11,%22center%22:{%22lat%22:36.560348944929686,%22lng%22:-87.34645450000001},%22city%22:[%22Clarksville%22],%22State%22:[%22TN%22]}";
            county = "Clarksville";
            GetHtml();
        }

        private void ListBoxItem_Cleveland(object sender, RoutedEventArgs e)
        {
            url = "https://www.remax.com/homes-for-sale/TN/Cleveland/city/4715400?filters={%22place%22:{%22lat%22:35.180622,%22lon%22:-84.886125,%22city%22:%22Cleveland%22,%22state%22:%22TN%22,%22placename%22:%22Cleveland,%20TN%22,%22placeType%22:%22city%22,%22placeId%22:%224715400%22,%22areaSquareMiles%22:26.82},%22locationRect%22:{%22minLat%22:35.241985,%22maxLat%22:35.11926,%22minLon%22:-84.967565,%22maxLon%22:-84.804685},%22bPropertyType%22:[%22Single%20Family%22,%22Condo/Townhome%22,%22Mobile%20Home%22,%22Multi-Family%22,%22Rental%22,%22Farm%22,%22Land%22],%22bStatus%22:[%22For%20Sale%22,%22Under%20Contract%22],%22city%22:[%22Cleveland%22],%22State%22:[%22TN%22],%22zoom%22:12,%22center%22:{%22lat%22:35.1806456628031,%22lng%22:-84.88612499999998}}";
            county = "Cleveland";
            GetHtml();
        }

        private void ListBoxItem_Collierville(object sender, RoutedEventArgs e)
        {
            url = "https://www.remax.com/homes-for-sale/TN/Collierville/city/4716420?filters={%22place%22:{%22lat%22:35.05841900019122,%22lon%22:-89.69253150048765,%22city%22:%22Collierville%22,%22state%22:%22TN%22,%22placename%22:%22Collierville,%20TN%22,%22placeType%22:%22city%22,%22placeId%22:%224716420%22,%22areaSquareMiles%22:29.53},%22locationRect%22:{%22minLat%22:35.110826,%22maxLat%22:35.006012,%22minLon%22:-89.744217,%22maxLon%22:-89.640846},%22bPropertyType%22:[%22Single%20Family%22,%22Condo/Townhome%22,%22Mobile%20Home%22,%22Multi-Family%22,%22Rental%22,%22Farm%22,%22Land%22],%22bStatus%22:[%22For%20Sale%22,%22Under%20Contract%22],%22city%22:[%22Collierville%22],%22State%22:[%22TN%22],%22zoom%22:12,%22center%22:{%22lat%22:35.05843581880185,%22lng%22:-89.69253150000003}}";
            county = "Collierville";
            GetHtml();
        }

        private void ListBoxItem_Cookeville(object sender, RoutedEventArgs e)
        {
            url = "https://www.remax.com/homes-for-sale/TN/Cookeville/city/4716920?filters={%22place%22:{%22lat%22:36.14045,%22lon%22:-85.515906,%22city%22:%22Cookeville%22,%22state%22:%22TN%22,%22placename%22:%22Cookeville,%20TN%22,%22placeType%22:%22city%22,%22placeId%22:%224716920%22,%22areaSquareMiles%22:32.75},%22locationRect%22:{%22minLat%22:36.200266,%22maxLat%22:36.080635,%22minLon%22:-85.58999,%22maxLon%22:-85.441823},%22bPropertyType%22:[%22Single%20Family%22,%22Condo/Townhome%22,%22Mobile%20Home%22,%22Multi-Family%22,%22Rental%22,%22Farm%22,%22Land%22],%22bStatus%22:[%22For%20Sale%22,%22Under%20Contract%22],%22city%22:[%22Cookeville%22],%22State%22:[%22TN%22],%22zoom%22:12,%22center%22:{%22lat%22:36.14047330201049,%22lng%22:-85.51590650000001}}";
            county = "Cookeville";
            GetHtml();
        }

        private void ListBoxItem_Columbia(object sender, RoutedEventArgs e)
        {
            url = "https://www.remax.com/homes-for-sale/TN/Columbia/city/4716540?filters={%22place%22:{%22lat%22:35.643898,%22lon%22:-87.012931,%22city%22:%22Columbia%22,%22state%22:%22TN%22,%22placename%22:%22Columbia,%20TN%22,%22placeType%22:%22city%22,%22placeId%22:%224716540%22,%22areaSquareMiles%22:31.51},%22locationRect%22:{%22minLat%22:35.722516,%22maxLat%22:35.565281,%22minLon%22:-87.140071,%22maxLon%22:-86.885791},%22bPropertyType%22:[%22Single%20Family%22,%22Condo/Townhome%22,%22Mobile%20Home%22,%22Multi-Family%22,%22Rental%22,%22Farm%22,%22Land%22],%22bStatus%22:[%22For%20Sale%22,%22Under%20Contract%22],%22city%22:[%22Columbia%22],%22State%22:[%22TN%22],%22zoom%22:11,%22center%22:{%22lat%22:35.64393717759466,%22lng%22:-87.012931}}";
            county = "Columbia";
            GetHtml();
        }

        private void ListBoxItem_Franklin(object sender, RoutedEventArgs e)
        {
            url = "https://www.remax.com/homes-for-sale/TN/Franklin/city/4727740?filters={%22place%22:{%22lat%22:35.904684,%22lon%22:-86.848889,%22city%22:%22Franklin%22,%22state%22:%22TN%22,%22placename%22:%22Franklin,%20TN%22,%22placeType%22:%22city%22,%22placeId%22:%224727740%22,%22areaSquareMiles%22:41.44},%22locationRect%22:{%22minLat%22:35.981503,%22maxLat%22:35.827865,%22minLon%22:-86.947863,%22maxLon%22:-86.749915},%22bPropertyType%22:[%22Single%20Family%22,%22Condo/Townhome%22,%22Mobile%20Home%22,%22Multi-Family%22,%22Rental%22,%22Farm%22,%22Land%22],%22bStatus%22:[%22For%20Sale%22,%22Under%20Contract%22],%22city%22:[%22Franklin%22],%22State%22:[%22TN%22],%22zoom%22:11,%22center%22:{%22lat%22:35.90472128429948,%22lng%22:-86.84888899999999}}";
            county = "Franklin";
            GetHtml();
        }

        private void ListBoxItem_Gallatin(object sender, RoutedEventArgs e)
        {
            url = "https://www.remax.com/homes-for-sale/TN/Gallatin/city/4728540?filters={%22place%22:{%22lat%22:36.381641,%22lon%22:-86.465716,%22city%22:%22Gallatin%22,%22state%22:%22TN%22,%22placename%22:%22Gallatin,%20TN%22,%22placeType%22:%22city%22,%22placeId%22:%224728540%22,%22areaSquareMiles%22:31.67},%22locationRect%22:{%22minLat%22:36.432592,%22maxLat%22:36.330691,%22minLon%22:-86.558888,%22maxLon%22:-86.372544},%22bPropertyType%22:[%22Single%20Family%22,%22Condo/Townhome%22,%22Mobile%20Home%22,%22Multi-Family%22,%22Rental%22,%22Farm%22,%22Land%22],%22bStatus%22:[%22For%20Sale%22,%22Under%20Contract%22],%22city%22:[%22Gallatin%22],%22State%22:[%22TN%22],%22zoom%22:11,%22center%22:{%22lat%22:36.38165819075099,%22lng%22:-86.46571600000003}}";
            county = "Gallatin";
            GetHtml();
        }

        private void ListBoxItem_Germantown(object sender, RoutedEventArgs e)
        {
            url = "https://www.remax.com/homes-for-sale/TN/Germantown/city/4728960?filters={%22place%22:{%22lat%22:35.074695,%22lon%22:-89.790864,%22city%22:%22Germantown%22,%22state%22:%22TN%22,%22placename%22:%22Germantown,%20TN%22,%22placeType%22:%22city%22,%22placeId%22:%224728960%22,%22areaSquareMiles%22:19.9},%22locationRect%22:{%22minLat%22:35.120996,%22maxLat%22:35.028394,%22minLon%22:-89.845606,%22maxLon%22:-89.736123},%22bPropertyType%22:[%22Single%20Family%22,%22Condo/Townhome%22,%22Mobile%20Home%22,%22Multi-Family%22,%22Rental%22,%22Farm%22,%22Land%22],%22bStatus%22:[%22For%20Sale%22,%22Under%20Contract%22],%22city%22:[%22Germantown%22],%22State%22:[%22TN%22],%22zoom%22:12,%22center%22:{%22lat%22:35.07470813589014,%22lng%22:-89.79086450000001}}";
            county = "GermanTown";
            GetHtml();
        }

        private void ListBoxItem_Hendersonville(object sender, RoutedEventArgs e)
        {
            url = "https://www.remax.com/homes-for-sale/TN/Hendersonville/city/4733280?filters={%22place%22:{%22lat%22:36.307029,%22lon%22:-86.598,%22city%22:%22Hendersonville%22,%22state%22:%22TN%22,%22placename%22:%22Hendersonville,%20TN%22,%22placeType%22:%22city%22,%22placeId%22:%224733280%22,%22areaSquareMiles%22:36.84},%22locationRect%22:{%22minLat%22:36.369854,%22maxLat%22:36.244204,%22minLon%22:-86.67942,%22maxLon%22:-86.516581},%22bPropertyType%22:[%22Single%20Family%22,%22Condo/Townhome%22,%22Mobile%20Home%22,%22Multi-Family%22,%22Rental%22,%22Farm%22,%22Land%22],%22bStatus%22:[%22For%20Sale%22,%22Under%20Contract%22],%22city%22:[%22Hendersonville%22],%22State%22:[%22TN%22],%22zoom%22:12,%22center%22:{%22lat%22:36.30705430808569,%22lng%22:-86.59800050000001}}";
            county = "Hendersonville";
            GetHtml();
        }

        private void ListBoxItem_Jackson(object sender, RoutedEventArgs e)
        {
            url = "https://www.remax.com/homes-for-sale/TN/Jackson/city/4737640?filters={%22place%22:{%22lat%22:35.648821,%22lon%22:-88.83033,%22city%22:%22Jackson%22,%22state%22:%22TN%22,%22placename%22:%22Jackson,%20TN%22,%22placeType%22:%22city%22,%22placeId%22:%224737640%22,%22areaSquareMiles%22:53.79},%22locationRect%22:{%22minLat%22:35.75707,%22maxLat%22:35.540226,%22minLon%22:-88.921076,%22maxLon%22:-88.739584},%22bPropertyType%22:[%22Single%20Family%22,%22Condo/Townhome%22,%22Mobile%20Home%22,%22Multi-Family%22,%22Rental%22,%22Farm%22,%22Land%22],%22bStatus%22:[%22For%20Sale%22,%22Under%20Contract%22],%22city%22:[%22Jackson%22],%22State%22:[%22TN%22],%22zoom%22:11,%22center%22:{%22lat%22:35.64872157533473,%22lng%22:-88.83033000000002}}";
            county = "Jackson";
            GetHtml();
        }

        private void ListBoxItem_JohnsonCity(object sender, RoutedEventArgs e)
        {
            url = "https://www.remax.com/homes-for-sale/TN/Johnson-City/city/4738320?filters={%22place%22:{%22lat%22:36.346859999908645,%22lon%22:-82.40805849979192,%22city%22:%22Johnson%20City%22,%22state%22:%22TN%22,%22placename%22:%22Johnson%20City,%20TN%22,%22placeType%22:%22city%22,%22placeId%22:%224738320%22,%22areaSquareMiles%22:43.11},%22locationRect%22:{%22minLat%22:36.434641,%22maxLat%22:36.259079,%22minLon%22:-82.523353,%22maxLon%22:-82.292764},%22bPropertyType%22:[%22Single%20Family%22,%22Condo/Townhome%22,%22Mobile%20Home%22,%22Multi-Family%22,%22Rental%22,%22Farm%22,%22Land%22],%22bStatus%22:[%22For%20Sale%22,%22Under%20Contract%22],%22city%22:[%22Johnson%20City%22],%22State%22:[%22TN%22],%22zoom%22:11,%22center%22:{%22lat%22:36.34690947983257,%22lng%22:-82.40805850000002}}";
            county = "Johnson_City";
            GetHtml();
        }

        private void ListBoxItem_Kingsport(object sender, RoutedEventArgs e)
        {
            url = "https://www.remax.com/homes-for-sale/TN/Kingsport/city/4739560?filters={%22place%22:{%22lat%22:36.514417499990465,%22lon%22:-82.52786549997737,%22city%22:%22Kingsport%22,%22state%22:%22TN%22,%22placename%22:%22Kingsport,%20TN%22,%22placeType%22:%22city%22,%22placeId%22:%224739560%22,%22areaSquareMiles%22:50.44},%22locationRect%22:{%22minLat%22:36.594605,%22maxLat%22:36.43423,%22minLon%22:-82.674039,%22maxLon%22:-82.381692},%22bPropertyType%22:[%22Single%20Family%22,%22Condo/Townhome%22,%22Mobile%20Home%22,%22Multi-Family%22,%22Rental%22,%22Farm%22,%22Land%22],%22bStatus%22:[%22For%20Sale%22,%22Under%20Contract%22],%22city%22:[%22Kingsport%22],%22State%22:[%22TN%22],%22zoom%22:11,%22center%22:{%22lat%22:36.51445904307114,%22lng%22:-82.52786549999999}}";
            county = "KingSport";
            GetHtml();
        }

        private void ListBoxItem_Knoxville(object sender, RoutedEventArgs e)
        {
            url = "https://www.remax.com/homes-for-sale/TN/Knoxville/city/4740000?filters={%22place%22:{%22lat%22:35.958362,%22lon%22:-83.925248,%22city%22:%22Knoxville%22,%22state%22:%22TN%22,%22placename%22:%22Knoxville,%20TN%22,%22placeType%22:%22city%22,%22placeId%22:%224740000%22,%22areaSquareMiles%22:104},%22locationRect%22:{%22minLat%22:36.067141,%22maxLat%22:35.849584,%22minLon%22:-84.161768,%22maxLon%22:-83.688729},%22bPropertyType%22:[%22Single%20Family%22,%22Condo/Townhome%22,%22Mobile%20Home%22,%22Multi-Family%22,%22Rental%22,%22Farm%22,%22Land%22],%22bStatus%22:[%22For%20Sale%22,%22Under%20Contract%22],%22city%22:[%22Knoxville%22],%22State%22:[%22TN%22],%22zoom%22:10,%22center%22:{%22lat%22:35.95843740857722,%22lng%22:-83.92524849999998}}";
            county = "Knoxville";
            GetHtml();
        }

        private void ListBoxItem_LaVergne(object sender, RoutedEventArgs e)
        {
            url = "https://www.remax.com/homes-for-sale/TN/La-Vergne/city/4741200?filters={%22place%22:{%22lat%22:36.021162,%22lon%22:-86.559722,%22city%22:%22La%20Vergne%22,%22state%22:%22TN%22,%22placename%22:%22La%20Vergne,%20TN%22,%22placeType%22:%22city%22,%22placeId%22:%224741200%22,%22areaSquareMiles%22:25.02},%22locationRect%22:{%22minLat%22:36.08776,%22maxLat%22:35.954565,%22minLon%22:-86.62452,%22maxLon%22:-86.494924},%22bPropertyType%22:[%22Single%20Family%22,%22Condo/Townhome%22,%22Mobile%20Home%22,%22Multi-Family%22,%22Rental%22,%22Farm%22,%22Land%22],%22bStatus%22:[%22For%20Sale%22,%22Under%20Contract%22],%22city%22:[%22La%20Vergne%22],%22State%22:[%22TN%22],%22zoom%22:12,%22center%22:{%22lat%22:36.021190642448204,%22lng%22:-86.55972200000002}}";
            county = "LaVergne";
            GetHtml();

        }

        private void ListBoxItem_Lebanon(object sender, RoutedEventArgs e)
        {
            url = "https://www.remax.com/homes-for-sale/TN/Lebanon/city/4741520?filters={%22place%22:{%22lat%22:36.187187,%22lon%22:-86.34908,%22city%22:%22Lebanon%22,%22state%22:%22TN%22,%22placename%22:%22Lebanon,%20TN%22,%22placeType%22:%22city%22,%22placeId%22:%224741520%22,%22areaSquareMiles%22:38.39},%22locationRect%22:{%22minLat%22:36.256209,%22maxLat%22:36.118166,%22minLon%22:-86.45402,%22maxLon%22:-86.244338},%22bPropertyType%22:[%22Single%20Family%22,%22Condo/Townhome%22,%22Mobile%20Home%22,%22Multi-Family%22,%22Rental%22,%22Farm%22,%22Land%22],%22bStatus%22:[%22For%20Sale%22,%22Under%20Contract%22],%22city%22:[%22Lebanon%22],%22State%22:[%22TN%22],%22zoom%22:11,%22center%22:{%22lat%22:36.18721791292234,%22lng%22:-86.349179}}";
            county = "Lebanon";
            GetHtml();
        }

        private void ListBoxItem_Memphis(object sender, RoutedEventArgs e)
        {
            url = "https://www.remax.com/homes-for-sale/TN/Memphis/city/4748000?filters={%22place%22:{%22lat%22:35.129127,%22lon%22:-89.969385,%22city%22:%22Memphis%22,%22state%22:%22TN%22,%22placename%22:%22Memphis,%20TN%22,%22placeType%22:%22city%22,%22placeId%22:%224748000%22,%22areaSquareMiles%22:323.46},%22locationRect%22:{%22minLat%22:35.264074,%22maxLat%22:34.99418,%22minLon%22:-90.301682,%22maxLon%22:-89.637089},%22bPropertyType%22:[%22Single%20Family%22,%22Condo/Townhome%22,%22Mobile%20Home%22,%22Multi-Family%22,%22Rental%22,%22Farm%22,%22Land%22],%22bStatus%22:[%22For%20Sale%22,%22Under%20Contract%22],%22city%22:[%22Memphis%22],%22State%22:[%22TN%22],%22zoom%22:10,%22center%22:{%22lat%22:35.12923881070498,%22lng%22:-89.9693855}}";
            county = "Memphis";
            GetHtml();
        }

        private void ListBoxItem_Murfreesboro(object sender, RoutedEventArgs e)
        {
            url = "https://www.remax.com/homes-for-sale/TN/Murfreesboro/city/4751560";
            county = "Murfreesboro";
            GetHtml();
        }

        private void ListBoxItem_MountJuliet(object sender, RoutedEventArgs e)
        {
            url = "https://www.remax.com/homes-for-sale/TN/Mount-Juliet/city/4750780?filters={%22place%22:{%22lat%22:36.215692,%22lon%22:-86.508922,%22city%22:%22Mount%20Juliet%22,%22state%22:%22TN%22,%22placename%22:%22Mount%20Juliet,%20TN%22,%22placeType%22:%22city%22,%22placeId%22:%224750780%22,%22areaSquareMiles%22:19.45},%22locationRect%22:{%22minLat%22:36.27776,%22maxLat%22:36.153624,%22minLon%22:-86.583831,%22maxLon%22:-86.434014},%22bPropertyType%22:[%22Single%20Family%22,%22Condo/Townhome%22,%22Mobile%20Home%22,%22Multi-Family%22,%22Rental%22,%22Farm%22,%22Land%22],%22bStatus%22:[%22For%20Sale%22,%22Under%20Contract%22],%22city%22:[%22Mount%20Juliet%22],%22State%22:[%22TN%22],%22zoom%22:12,%22center%22:{%22lat%22:36.21571661943813,%22lng%22:-86.50892250000001}}";
            county = "MtJuliet";
            GetHtml();
        }

        private void ListBoxItem_Nashville(object sender, RoutedEventArgs e)
        {
            url = "https://www.remax.com/homes-for-sale/TN/Nashville/city/4752006?filters={%22place%22:{%22lat%22:36.18663599998504,%22lon%22:-86.78512649996466,%22city%22:%22Nashville%22,%22state%22:%22TN%22,%22placename%22:%22Nashville,%20TN%22,%22placeType%22:%22city%22,%22placeId%22:%224752006%22,%22areaSquareMiles%22:496.69},%22locationRect%22:{%22minLat%22:36.405448,%22maxLat%22:35.967824,%22minLon%22:-87.054665,%22maxLon%22:-86.515588},%22bPropertyType%22:[%22Single%20Family%22,%22Condo/Townhome%22,%22Mobile%20Home%22,%22Multi-Family%22,%22Rental%22,%22Farm%22,%22Land%22],%22bStatus%22:[%22For%20Sale%22,%22Under%20Contract%22],%22city%22:[%22Nashville%22],%22State%22:[%22TN%22],%22zoom%22:10,%22center%22:{%22lat%22:36.186941651129445,%22lng%22:-86.78512649999999}}";
            county = "Nashville";
            GetHtml();
        }

        private void ListBoxItem_SpringHill(object sender, RoutedEventArgs e)
        {
            url = "https://www.remax.com/homes-for-sale/TN/Spring-Hill/city/4770580?filters={%22place%22:{%22lat%22:35.74242699955816,%22lon%22:-86.92382249892408,%22city%22:%22Spring%20Hill%22,%22state%22:%22TN%22,%22placename%22:%22Spring%20Hill,%20TN%22,%22placeType%22:%22city%22,%22placeId%22:%224770580%22,%22areaSquareMiles%22:27.02},%22locationRect%22:{%22minLat%22:35.79496,%22maxLat%22:35.689894,%22minLon%22:-86.990726,%22maxLon%22:-86.856919},%22bPropertyType%22:[%22Single%20Family%22,%22Condo/Townhome%22,%22Mobile%20Home%22,%22Multi-Family%22,%22Rental%22,%22Farm%22,%22Land%22],%22bStatus%22:[%22For%20Sale%22,%22Under%20Contract%22],%22city%22:[%22Spring%20Hill%22],%22State%22:[%22TN%22],%22zoom%22:12,%22center%22:{%22lat%22:35.74244433250235,%22lng%22:-86.92382250000001}}";
            county = "SpringHill";
            GetHtml();
        }

        private void ListBoxItem_Smyrna(object sender, RoutedEventArgs e)
        {
            url = "https://www.remax.com/homes-for-sale/TN/Smyrna/city/4769420?filters={%22place%22:{%22lat%22:35.972716,%22lon%22:-86.542762,%22city%22:%22Smyrna%22,%22state%22:%22TN%22,%22placename%22:%22Smyrna,%20TN%22,%22placeType%22:%22city%22,%22placeId%22:%224769420%22,%22areaSquareMiles%22:29.39},%22locationRect%22:{%22minLat%22:36.029197,%22maxLat%22:35.916236,%22minLon%22:-86.624984,%22maxLon%22:-86.46054},%22bPropertyType%22:[%22Single%20Family%22,%22Condo/Townhome%22,%22Mobile%20Home%22,%22Multi-Family%22,%22Rental%22,%22Farm%22,%22Land%22],%22bStatus%22:[%22For%20Sale%22,%22Under%20Contract%22],%22city%22:[%22Smyrna%22],%22State%22:[%22TN%22],%22zoom%22:12,%22center%22:{%22lat%22:35.97273670555324,%22lng%22:-86.54276199999998}}";
            county = "Smyrna";
            GetHtml();

        }

        private void ExitMessage_TextChanged(object sender, TextChangedEventArgs e) { }

        private void ListBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) { }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            exitMessage.Text = "Scraping Complete, the file has " +
                                  "been made in the documents folder " +
                                  "and it is called :" + county + ".txt";
        }
    }
}

