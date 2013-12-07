package com.mookie.prototype.donations;

import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.support.v4.app.Fragment;
import android.view.LayoutInflater;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.ViewGroup;
import android.widget.Button;

import com.mookie.android.ui.animation.SquashAndStretch;
import com.mookie.prototype.R;
import com.mookie.prototype.YourApplication;

import javax.inject.Inject;

public class DonateFragment extends Fragment implements OnClickListener {

    @Inject
    SquashAndStretch squashAndStretchAnim;

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        ((YourApplication) getActivity().getApplication()).inject(this);
    }

    @Override
	public View onCreateView(LayoutInflater inflater, ViewGroup container,
			Bundle savedInstanceState) {
		
		final View view = inflater.inflate(R.layout.donations_fragment, container, false);
		final Button button = (Button) view.findViewById(R.id.donations_paypal_donate_button);
        button.setOnClickListener(this);

        view.post( new Runnable() {
            @Override
            public void run() {
                squashAndStretchAnim.animate(button, view, 300, 1);
            }
        });
		return view;
	}
    @Override
	public void onClick(View v) {
        Uri.Builder uriBuilder = new Uri.Builder();
        uriBuilder.scheme("https").authority("www.paypal.com").path("cgi-bin/webscr");
        uriBuilder.appendQueryParameter("cmd", "_donations");

        uriBuilder.appendQueryParameter("business", "lmichenaud@gmail.com");
        uriBuilder.appendQueryParameter("lc", "US");
        uriBuilder.appendQueryParameter("item_name", "Project_X");
        uriBuilder.appendQueryParameter("no_note", "1");
        uriBuilder.appendQueryParameter("no_shipping", "1");
        uriBuilder.appendQueryParameter("currency_code", "EUR");
        Uri payPalUri = uriBuilder.build();
    	
       	Intent viewIntent = new Intent(Intent.ACTION_VIEW, payPalUri);
       	startActivity(viewIntent);
	}
}
