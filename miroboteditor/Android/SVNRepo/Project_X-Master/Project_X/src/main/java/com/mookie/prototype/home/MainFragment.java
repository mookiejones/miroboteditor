package com.mookie.prototype.home;

import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;

import com.mookie.android.ui.animation.LiveButton;
import com.mookie.android.ui.navdrawer.AbstractNavDrawerActivity;
import com.mookie.android.ui.navdrawer.NavDrawerSelectItemRunnable;
import com.mookie.prototype.R;
import com.mookie.prototype.YourApplication;

import javax.inject.Inject;

public class MainFragment extends Fragment {

    @Inject
    LiveButton liveButton ;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        ((YourApplication) getActivity().getApplication()).inject(this);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {

        View mainView = inflater
                .inflate(R.layout.main_fragment, container, false);

        Button button0 = (Button) mainView.findViewById(R.id.mainmenu_button0);
        liveButton.setupLiveAnimOnButton(button0, new NavDrawerSelectItemRunnable((AbstractNavDrawerActivity)this.getActivity(), 1));

        Button button1 = (Button) mainView.findViewById(R.id.mainmenu_button1);
        liveButton.setupLiveAnimOnButton(button1, new NavDrawerSelectItemRunnable((AbstractNavDrawerActivity)this.getActivity(), 2));

        Button button2 = (Button) mainView.findViewById(R.id.mainmenu_button2);
        liveButton.setupLiveAnimOnButton(button2, new NavDrawerSelectItemRunnable((AbstractNavDrawerActivity)this.getActivity(), 3));

        Button button3 = (Button) mainView.findViewById(R.id.mainmenu_button3);
        liveButton.setupLiveAnimOnButton(button3, new NavDrawerSelectItemRunnable((AbstractNavDrawerActivity)this.getActivity(), 4));

        return mainView;
    }
}
