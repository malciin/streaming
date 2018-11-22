import React from 'react';
import './indexPage.css'
import Navbar from '../../components/navbar/navbar';
import VideoPlayer from '../../components/blocks/videoPlayer/videoPlayer';
import { Config } from '../../shared/config';

class IndexPage extends React.Component{
    render() {
        return (
            <div className="indexPage">
                <Navbar />

                <div className="body">
                    <p>
                        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Quisque non magna congue, sodales erat a, molestie libero. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Sed et ornare erat. Sed vel viverra dui. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia Curae; Donec faucibus tellus libero, eu dignissim turpis pulvinar ac. Nam mauris purus, consectetur quis molestie eget, pretium nec lorem. Cras facilisis tortor vitae lectus scelerisque posuere. Cras scelerisque nibh orci, ac ultrices nunc porttitor nec. Sed non venenatis lacus. Etiam non vestibulum nisl. Donec pellentesque ultricies neque, et malesuada justo molestie eu. Duis velit neque, euismod et lacus ut, euismod vestibulum ligula. Ut facilisis sit amet ante at interdum. Ut ipsum ligula, venenatis vel nibh at, tincidunt iaculis turpis.
                    </p>
                    <p>
                        Nunc laoreet nisi sit amet orci elementum consectetur. Praesent id erat in urna faucibus consectetur. Sed sed pharetra diam. Curabitur consequat, libero nec scelerisque viverra, dolor turpis pretium dolor, non lobortis lectus sem nec erat. Quisque feugiat ultrices ipsum, sit amet eleifend urna porttitor at. Sed convallis orci ut sapien ultrices, eu dictum nisl congue. Proin varius tincidunt velit sit amet hendrerit. Nunc congue massa ac vehicula volutpat. Curabitur tincidunt libero id venenatis venenatis. Donec at pharetra mi, a egestas libero.
                    </p>
                        Vivamus ligula velit, varius at vehicula vel, ultrices at nisl. Nulla nibh neque, pharetra vel iaculis molestie, venenatis id ex. Etiam vel elementum mauris. Sed at sodales nisi. Aenean mattis pharetra ipsum, a semper lorem suscipit nec. Donec sagittis et leo non auctor. Nam lorem neque, sagittis viverra convallis in, mollis sed lorem.
                    <p>
                        Quisque sed nibh venenatis, dictum risus vitae, porta orci. Donec sit amet turpis lobortis, tempus tortor et, pretium elit. Cras finibus, est ut eleifend auctor, dui purus condimentum metus, sed suscipit lacus ante sit amet tellus. Nunc quam nisl, consequat nec auctor in, sagittis eget lorem. Proin dignissim nibh non tempus gravida. Etiam suscipit efficitur orci eu commodo. Suspendisse sem nulla, molestie eu maximus et, dictum vel arcu. Praesent sed commodo est. Nam luctus ligula magna, in dignissim tortor aliquam sit amet. Proin ipsum quam, efficitur nec venenatis ut, accumsan vel turpis. Duis tempor semper tellus vitae pretium. Cras pretium ac lorem et dapibus. Vivamus egestas lacinia risus id molestie. Proin vitae ligula at erat aliquam congue.
                    </p>
                    <p>
                        Morbi nec urna nibh. Sed gravida nisl ligula, id gravida magna imperdiet eget. Quisque erat sem, faucibus ac porttitor vitae, placerat ac ipsum. Curabitur sit amet justo urna. Sed et tempus elit. Pellentesque habitant morbi tristique senectus et netus et malesuada fames ac turpis egestas. Quisque scelerisque sagittis enim vitae laoreet. Proin finibus malesuada dolor, eget tincidunt ante auctor in. Aenean at rhoncus lorem. Ut ac ornare nisi. 
                    </p>

                    <VideoPlayer videoId="ceaea1ed-519b-414b-9e0d-79678ca2adcd" manifestEndpoint={`${Config.apiPath}/Video/Manifest`} />
                </div>
            </div>
        );
    }
}

export default IndexPage;